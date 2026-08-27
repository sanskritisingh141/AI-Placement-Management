from __future__ import annotations

import io
import re
from typing import Annotated

from fastapi import FastAPI, File, Form, HTTPException, UploadFile
from pydantic import BaseModel, Field
from pypdf import PdfReader
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import cosine_similarity


app = FastAPI(title="AI Placement Analysis Service", version="1.0.0")


class MatchRequest(BaseModel):
    resume_text: str = ""
    job_description: str = ""
    resume_skills: list[str] = Field(default_factory=list)
    required_skills: list[str] = Field(default_factory=list)


class SkillResult(BaseModel):
    name: str
    confidence: float


class ResumeAnalysisResponse(BaseModel):
    extracted_text: str
    summary: str
    skills: list[SkillResult]
    model_version: str = "tfidf-keywords-v1"


class MatchResponse(BaseModel):
    match_score: float
    matched_skills: list[str]
    missing_skills: list[str]
    recommendation: str
    model_version: str = "tfidf-keywords-v1"


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "healthy"}


@app.post("/analyze-resume", response_model=ResumeAnalysisResponse)
async def analyze_resume(
    file: Annotated[UploadFile, File()],
    known_skills: Annotated[str, Form()] = "",
) -> ResumeAnalysisResponse:
    if file.content_type != "application/pdf":
        raise HTTPException(status_code=400, detail="Only PDF files are accepted.")

    content = await file.read()
    if not content.startswith(b"%PDF-"):
        raise HTTPException(status_code=400, detail="Invalid PDF signature.")

    try:
        reader = PdfReader(io.BytesIO(content))
        text = "\n".join(page.extract_text() or "" for page in reader.pages).strip()
    except Exception as exc:
        raise HTTPException(status_code=400, detail="Unable to parse PDF.") from exc

    normalized_text = re.sub(r"\s+", " ", text)
    skill_names = [item.strip() for item in known_skills.split("|") if item.strip()]
    found = [
        SkillResult(name=name, confidence=0.95)
        for name in skill_names
        if re.search(rf"(?<!\w){re.escape(name)}(?!\w)", normalized_text, re.IGNORECASE)
    ]

    summary = normalized_text[:500]
    return ResumeAnalysisResponse(
        extracted_text=normalized_text,
        summary=summary,
        skills=found,
    )


@app.post("/match", response_model=MatchResponse)
def match_job(request: MatchRequest) -> MatchResponse:
    resume_skills = {skill.casefold(): skill for skill in request.resume_skills}
    required = {skill.casefold(): skill for skill in request.required_skills}
    matched_keys = sorted(resume_skills.keys() & required.keys())
    missing_keys = sorted(required.keys() - resume_skills.keys())

    skill_score = 1.0 if not required else len(matched_keys) / len(required)
    semantic_score = 0.0
    if request.resume_text.strip() and request.job_description.strip():
        vectors = TfidfVectorizer(stop_words="english").fit_transform(
            [request.resume_text, request.job_description]
        )
        semantic_score = float(cosine_similarity(vectors[0:1], vectors[1:2])[0][0])

    score = round((0.7 * skill_score + 0.3 * semantic_score) * 100, 2)
    missing = [required[key] for key in missing_keys]
    recommendation = (
        "Your profile strongly matches this role."
        if score >= 75
        else "Improve the missing required skills: " + ", ".join(missing)
        if missing
        else "Tailor your resume language to the job description."
    )

    return MatchResponse(
        match_score=score,
        matched_skills=[required[key] for key in matched_keys],
        missing_skills=missing,
        recommendation=recommendation,
    )
