
from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.responses import JSONResponse
from typing import Dict, Any, List, Optional
from openai_query import QuestionGenerator
from vector_db import get_vector_db
from file_processing import FileProcessor
import logging
import os
import asyncio
from concurrent.futures import ThreadPoolExecutor

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

app = FastAPI(
    title="Hệ thống sinh câu hỏi trắc nghiệm thông minh",
    description="API sinh câu hỏi trắc nghiệm với khả năng xử lý song song nhiều yêu cầu",
    version="1.0.0"
)

# Initialize components
vector_db = get_vector_db()
question_generator = QuestionGenerator(
    deepseek_api_key="sk-e2229e4821f94ef99c0cb09ace4c5050",
    openai_api_key="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",
    chroma_directory="./chroma_db"
)

# Thread pool for CPU-bound tasks
thread_pool = ThreadPoolExecutor(max_workers=10)

def validate_and_extract(data: Dict[str, Any], required_fields: List[str]) -> tuple:
    """Validate input data and extract values"""
    missing_fields = [field for field in required_fields if field not in data]
    if missing_fields:
        return False, f"Missing required fields: {', '.join(missing_fields)}", None
    return True, "", {field: data[field] for field in required_fields}

async def run_in_threadpool(func, *args):
    """Run synchronous functions in threadpool"""
    loop = asyncio.get_event_loop()
    return await loop.run_in_executor(thread_pool, func, *args)

@app.get("/", response_model=Dict[str, Any])
async def home():
    """List available collections and endpoints"""
    collections = await run_in_threadpool(vector_db.list_collections)
    return {
        "message": "Hệ thống sinh câu hỏi trắc nghiệm thông minh",
        "available_collections": collections,
        "endpoints": {
            "/generate-mcq": "Sinh câu hỏi trắc nghiệm cơ bản",
            "/generate-adaptive-question": "Sinh câu hỏi thích ứng theo hiệu suất học tập",
            "/generate-batch-questions": "Sinh số lượng lớn câu hỏi theo batch",
            "/upload-file": "Tải lên và xử lý tài liệu"
        }
    }

@app.post("/generate-mcq", response_model=Dict[str, Any])
async def generate_mcq(data: Dict[str, Any]):
    """Generate multiple choice questions"""
    try:
        is_valid, error_msg, extracted = validate_and_extract(data, ["question"])
        if not is_valid:
            raise HTTPException(status_code=400, detail=error_msg)

        user_question = extracted["question"].strip()
        num_questions = int(data.get("num_questions", 3))
        collection_name = data.get("collection_name", "default")

        # Check collection exists
        collection_exists = await run_in_threadpool(vector_db.collection_exists, collection_name)
        if not collection_exists:
            logger.warning(f"Collection '{collection_name}' không tồn tại")

        # Generate questions (async)
        questions = await run_in_threadpool(
            lambda: question_generator.generate_questions(
                query={
                    "question": user_question,
                    "num_questions": num_questions
                },
                collection_name=collection_name,
                difficulty_mode='random'
            )
        )

        return {"questions": questions}

    except Exception as e:
        logger.exception("Lỗi tại /generate-mcq")
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/generate-adaptive-question", response_model=Dict[str, Any])
async def generate_adaptive_question(data: Dict[str, Any]):
    """Generate adaptive questions based on performance"""
    try:
        is_valid, error_msg, extracted = validate_and_extract(data, ["subject", "question"])
        if not is_valid:
            raise HTTPException(status_code=400, detail=error_msg)

        subject = extracted["subject"].strip()
        user_question = extracted["question"].strip()
        num_questions = int(data.get("num_questions", 3))
        collection_name = data.get("collection_name", subject.lower())
        results = data.get("results", {})

        # Check collection exists
        collection_exists = await run_in_threadpool(vector_db.collection_exists, collection_name)
        if not collection_exists:
            logger.warning(f"Collection '{collection_name}' không tồn tại")

        # Generate questions (async)
        from functools import partial

        # Đóng gói sẵn hàm và các đối số
        func = partial(
            question_generator.generate_questions,
            query={
                "subject": subject,
                "question": user_question,
                "num_questions": num_questions,
                "results": {
                    "num_correct": int(results.get("num_correct", 0)),
                    "total_questions": int(results.get("total_questions", 1)),
                    "time_taken": float(results.get("time_taken", 0)),
                    "previous_level": int(results.get("previous_level", 2))
                }
            },
            collection_name=collection_name,
            difficulty_mode="similar"
        )

        # Gọi hàm qua threadpool
        questions = await run_in_threadpool(func)


        response = {"questions": questions}
        if not collection_exists:
            response["note"] = f"Chưa có kiến thức chuyên biệt cho '{collection_name}'"

        return response

    except Exception as e:
        logger.exception("Lỗi tại /generate-adaptive-question")
        raise HTTPException(status_code=500, detail=str(e))


@app.post("/generate-batch-questions", response_model=Dict[str, Any])
async def generate_batch_questions(data: Dict[str, Any]):
    """Generate large batch of questions"""
    try:
        is_valid, error_msg, extracted = validate_and_extract(data, ["question"])
        if not is_valid:
            raise HTTPException(status_code=400, detail=error_msg)

        user_question = extracted["question"].strip()
        num_questions = int(data.get("num_questions", 20))
        collection_name = data.get("collection_name", "default")

        # Check collection exists
        collection_exists = await run_in_threadpool(vector_db.collection_exists, collection_name)
        if not collection_exists:
            logger.warning(f"Collection '{collection_name}' không tồn tại")

        # Generate questions (async)
        questions = await run_in_threadpool(
            lambda: question_generator.generate_large_batch,
            query={
                "question": user_question,
                "num_questions": num_questions
            },
            collection_name=collection_name
        )

        response = {"questions": questions}
        if not collection_exists:
            response["note"] = f"Chưa có kiến thức chuyên biệt cho '{collection_name}'"

        return response

    except Exception as e:
        logger.exception("Lỗi tại /generate-batch-questions")
        raise HTTPException(status_code=500, detail=str(e))

from typing import List

@app.post("/upload-file", response_model=Dict[str, Any])
async def upload_file(files: List[UploadFile] = File(...)):
    """Upload and process multiple document files"""
    try:
        os.makedirs("uploads", exist_ok=True)
        results = {}

        for file in files:
            file_path = os.path.join("uploads", file.filename)
            with open(file_path, "wb") as buffer:
                buffer.write(await file.read())

            processor = FileProcessor()
            documents = await run_in_threadpool(processor.process_file, file_path)

            if not documents:
                logger.warning(f"Không có đoạn văn bản nào từ file {file.filename}")
                continue

            collection_name = os.path.splitext(file.filename)[0]
            await run_in_threadpool(vector_db.add_documents, documents, collection_name)

            logger.info(f"Đã xử lý và lưu '{file.filename}' ({len(documents)} đoạn văn)")
            results[file.filename] = {
                "collection_name": collection_name,
                "documents_processed": len(documents)
            }

        if not results:
            raise HTTPException(status_code=400, detail="Không có file nào được xử lý thành công")

        return {
            "message": "Đã xử lý thành công các file",
            "results": results
        }

    except Exception as e:
        logger.exception("Lỗi tại /upload-file")
        raise HTTPException(status_code=500, detail=str(e))


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(
        app,
        host="0.0.0.0",
        port=5000,
        workers=5,  # Số worker processes (tận dụng đa CPU core)
        loop="uvloop",  # Sử dụng uvloop để hiệu suất tốt hơn
        http="httptools"  # HTTP parser nhanh
    )