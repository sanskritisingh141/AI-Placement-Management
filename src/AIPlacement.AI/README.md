# AI Placement Analysis Service

Run locally:

```powershell
python -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
uvicorn app.main:app --reload --port 8000
```

The ASP.NET API expects `AIService:BaseUrl` to point to this service.
