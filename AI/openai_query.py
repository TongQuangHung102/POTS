import json
import re
import os
import logging
import random
from typing import List, Dict, Any, Optional, Union
from langchain.prompts import PromptTemplate
from langchain_openai import ChatOpenAI
from langchain.chains import RetrievalQA
from langchain_community.vectorstores import FAISS, Chroma
from langchain_openai import OpenAIEmbeddings
from langchain_openai import OpenAI
from langchain_deepseek import ChatDeepSeek
from vector_db import get_vector_db  

# Set up logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# Lấy API key từ biến môi trường (an toàn hơn)
openai_api_key = "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"
deepseek_api_key = "sk-e2229e4821f94ef99c0cb09ace4c5050"
vector_db = get_vector_db()

class QuestionGenerator:
    def __init__(self, openai_api_key: str, deepseek_api_key: str, chroma_directory: str = "./chroma_db"):
        self.openai_api_key = openai_api_key
        self.deepseek_api_key = deepseek_api_key
        self.chroma_directory = chroma_directory
        self.embeddings = OpenAIEmbeddings(api_key=self.openai_api_key)

        # Prompt random với yếu tố ngẫu nhiên
        self.prompt_random = PromptTemplate.from_template("""
Context: {context}

Hãy suy nghĩ và tạo ra {num_questions} câu hỏi trắc nghiệm với MỨC ĐỘ KHÁC NHAU về chủ đề ở trên với 4 đáp án (A, B, C, D) cho mỗi câu hỏi. Các câu hỏi này nên **tương tự** những câu hỏi trong sách giáo khoa nhưng được **diễn đạt khác** và **được thay đổi một chút về số liệu hoặc cách thức ra đề**.

**Yêu cầu đặc biệt:**
- Câu hỏi không được giống y hệt với bất kỳ câu hỏi nào đã có.
- Các câu hỏi cần có sự đa dạng về mức độ khó (1-4).
- Hãy đảm bảo các câu hỏi có sự thay đổi nhỏ về nội dung nhưng vẫn giữ được tính chất của chủ đề.

Seed: {random_seed}

CHÚ Ý:
- Phân bố mức độ câu hỏi từ dễ đến khó (1-4).
- Định dạng JSON PHẢI ĐÚNG CÚ PHÁP không thiếu dấu phẩy, ngoặc.
- LUÔN trả về duy nhất một mảng JSON trong dấu ngoặc vuông [ ].
- ĐẢM BẢO TẤT CẢ các phần tử trong mảng answerQuestions đều được phân tách bằng dấu phẩy.
- Không thêm bất kỳ văn bản giải thích nào ngoài mảng JSON.
- Sử dụng phong cách viết câu hỏi: {question_style}

Mỗi câu hỏi PHẢI được định dạng như sau:
{{
    "questionText": "Nội dung câu hỏi", 
    "levelId": {level_id}, 
    "answerQuestions": [
        {{"answerText": "Đáp án A", "number": 1}}, 
        {{"answerText": "Đáp án B", "number": 2}},
        {{"answerText": "Đáp án C", "number": 3}}, 
        {{"answerText": "Đáp án D", "number": 4}}
    ],
    "correctAnswer": số từ 1 đến 4 chỉ đáp án đúng
}}

TRẢ VỀ mảng JSON của {num_questions} câu hỏi.

""")

        # Prompt adaptive với phân tích học sinh chi tiết hơn
        self.prompt_similar_level = PromptTemplate.from_template("""
Context: {context}

Dựa trên dữ liệu học sinh cụ thể:                                                                
- Chủ đề: {subject}
- Dạng bài tập mẫu: {question}
- Số câu hỏi cần sinh: {num_questions}
- Số câu trả lời đúng: {num_correct}/{total_questions} (tỉ lệ: {ratio_correct:.0f}%)
- Thời gian trung bình mỗi câu: {avg_time_per_question:.2f} giây
- Lịch sử mức độ trước đó: {previous_level}
- Đặc điểm học sinh: {student_characteristic}
- Seed ngẫu nhiên: {random_seed}
- Câu hỏi phải rõ ràng, ngắn gọn,không cần có chú thích, không có ký tự đặc biệt như (), [], v.v ở cuối câu.
- Kết thúc câu hỏi bằng dấu chấm hỏi "?".

PHÂN TÍCH MỨC ĐỘ:
1. Tiêu chí tỉ lệ đúng:
- ≥80% → Tăng độ khó
- 50-79% → Giữ nguyên
- <50% → Giảm độ khó
2. Tiêu chí thời gian:
- <10s/câu → Tăng độ khó
- >30s/câu → Giảm độ khó
3. Quy tắc điều chỉnh:
- Nếu cả 2 tiêu chí cùng đề xuất tăng → Tăng 2 mức
- Nếu 1 tiêu chí đề xuất tăng → Tăng 1 mức
- Tương tự với giảm độ khó
- Mức độ mới phải nằm trong khoảng 1-4

YÊU CẦU:
- Dựa trên chủ đề: {subject}
- Kế thừa dạng bài từ: {question}
- Sinh {num_questions} câu hỏi toán học
- Mức độ mới: {new_level}
- Tùy chỉnh phong cách câu hỏi: {question_style}

ĐỊNH DẠNG JSON:
[{{
"questionText": "Nội dung câu hỏi",
"levelId": {new_level},
"answerQuestions": [
    {{"answerText": "A", "number": 1}},
    {{"answerText": "B", "number": 2}},
    {{"answerText": "C", "number": 3}},
    {{"answerText": "D", "number": 4}}
],
"correctAnswer": 1
}}]
""")

    def create_llm(self, model: str = "deepseek-chat", max_tokens: int = 2000, temperature: float = 0.9) -> Union[ChatDeepSeek, ChatOpenAI]:

        """Ưu tiên tạo LLM bằng DeepSeek nếu có API Key, fallback sang OpenAI"""
        try:
            # Thêm biến động cho temperature để tăng đa dạng
            adjusted_temperature = temperature + random.uniform(-0.1, 0.2)
            adjusted_temperature = max(0.4, min(1.0, adjusted_temperature))  # Giới hạn trong khoảng 0.4 - 1.0
            
            if self.deepseek_api_key:
                logger.info(f"✅ Sử dụng DeepSeek: {model} (temperature: {adjusted_temperature:.2f})")
                return ChatDeepSeek(
                    model=model,
                    api_key=self.deepseek_api_key,
                    max_tokens=max_tokens,
                    temperature=adjusted_temperature
                )
            elif self.openai_api_key:
                logger.info(f"🔁 Fallback sang OpenAI: {model} (temperature: {adjusted_temperature:.2f})")
                return ChatOpenAI(
                    model="gpt-4",
                    openai_api_key=self.openai_api_key,
                    max_tokens=max_tokens,
                    temperature=adjusted_temperature
                )
            else:
                raise ValueError("❌ Không có API key hợp lệ.")
        except Exception as e:
            logger.warning(f"⚠️ Lỗi khi tạo LLM: {e}")
            if self.openai_api_key:
                logger.info("🔁 Fallback cuối cùng sang OpenAI GPT-4")
                return ChatOpenAI(
                    model="gpt-4",
                    openai_api_key=self.openai_api_key,
                    max_tokens=max_tokens,
                    temperature=0.7
                )
            raise

    def create_retriever(self, collection_name: str = "default"):
        try:
            collection_path = f"{self.chroma_directory}/{collection_name}"
            db = Chroma(
                persist_directory=collection_path,
                embedding_function=self.embeddings
            )
            logger.info(f"Đã tạo retriever từ ChromaDB tại {collection_path}")
            return db.as_retriever(search_kwargs={"k": 5})
        except Exception as e:
            logger.warning(f"Không thể tạo retriever từ ChromaDB: {e}")
            try:
                faiss_index = FAISS.load_local(f"path/to/faiss_index/{collection_name}", self.embeddings)
                logger.info(f"Đã tạo retriever từ FAISS")
                return faiss_index.as_retriever(search_kwargs={"k": 5})
            except Exception as e2:
                logger.error(f"Không thể tạo retriever từ FAISS: {e2}")
                return None

    def get_student_characteristic(self, results: Dict[str, Any]) -> str:
        """
        Phân tích kết quả và sinh ra mô tả đặc điểm học sinh để tăng tính đa dạng
        """
        num_correct = results.get('num_correct', 0)
        total_questions = results.get('total_questions', 0)
        time_taken = results.get('time_taken', 0)
        previous_level = results.get('previous_level', 2)
        
        ratio_correct = (num_correct / total_questions * 100) if total_questions > 0 else 0
        avg_time_per_question = time_taken / total_questions if total_questions > 0 else 0
        
        characteristics = []
        
        # Phân tích về độ chính xác
        if ratio_correct >= 90:
            characteristics.append(random.choice([
                "có khả năng tiếp thu kiến thức rất tốt",
                "nắm vững kiến thức cơ bản và nâng cao",
                "thường xuyên làm đúng các bài tập",
                "thường đạt kết quả xuất sắc"
            ]))
        elif ratio_correct >= 70:
            characteristics.append(random.choice([
                "nắm tốt kiến thức cơ bản",
                "có khả năng giải quyết các bài toán trung bình",
                "đôi khi mắc lỗi ở những câu khó",
                "có thể tiếp thu kiến thức mới khá tốt"
            ]))
        elif ratio_correct >= 50:
            characteristics.append(random.choice([
                "nắm được một số kiến thức cơ bản",
                "có thể giải quyết các bài toán đơn giản",
                "đôi khi gặp khó khăn với bài tập phức tạp",
                "cần củng cố thêm kiến thức"
            ]))
        else:
            characteristics.append(random.choice([
                "đang gặp khó khăn trong việc nắm bắt kiến thức",
                "cần được hỗ trợ nhiều hơn với các bài tập cơ bản",
                "thường xuyên mắc lỗi khi giải toán",
                "cần thời gian để củng cố kiến thức nền tảng"
            ]))
        
        # Phân tích về tốc độ làm bài
        if avg_time_per_question < 10:
            characteristics.append(random.choice([
                "làm bài rất nhanh",
                "có khả năng xử lý thông tin nhanh",
                "không gặp khó khăn với thời gian",
                "có thể tăng độ khó để thách thức"
            ]))
        elif avg_time_per_question < 20:
            characteristics.append(random.choice([
                "làm bài với tốc độ khá",
                "cân bằng giữa tốc độ và độ chính xác",
                "xử lý thông tin ở mức trung bình",
                "có thể cải thiện tốc độ xử lý"
            ]))
        else:
            characteristics.append(random.choice([
                "cần nhiều thời gian để giải quyết bài toán",
                "xử lý thông tin chậm hơn so với mức tiêu chuẩn",
                "thường suy nghĩ kỹ trước khi đưa ra đáp án",
                "cần rèn luyện thêm để cải thiện tốc độ"
            ]))
        
        # Thêm đặc điểm ngẫu nhiên
        random_characteristics = [
            "thường thích các bài toán ứng dụng thực tế",
            "có thể học tốt hơn với hình ảnh trực quan",
            "đôi khi gặp khó khăn với các phương pháp trừu tượng",
            "thích thử thách với các bài toán mới",
            "thường thích các bài toán có cấu trúc rõ ràng",
            "có thể học tốt hơn thông qua các ví dụ",
            "đôi khi cần thêm hướng dẫn chi tiết",
            "có tiềm năng phát triển tốt nếu được hướng dẫn đúng cách"
        ]
        characteristics.append(random.choice(random_characteristics))
        
        return ", ".join(characteristics)

    def get_question_style(self) -> str:
        """
        Tạo phong cách câu hỏi ngẫu nhiên để tăng tính đa dạng
        """
        styles = [
            "thiết thực và gắn với tình huống thực tế",
            "truyền thống và đi thẳng vào vấn đề",
            "khuyến khích tư duy phân tích",
            "tập trung vào khái niệm cơ bản",
            "yêu cầu áp dụng kiến thức vào tình huống mới",
            "kết hợp kiến thức từ nhiều phần khác nhau",
            "sử dụng ngôn ngữ đơn giản và dễ hiểu",
            "có yếu tố thử thách phù hợp với trình độ học sinh",
            "sử dụng các ví dụ thực tế để minh họa",
            "khuyến khích tư duy sáng tạo và linh hoạt"
        ]
        return random.choice(styles)

    def get_enhanced_relevant_context(self, query: str, collection_name: str = "default", k: int = 5) -> str:
        """
        Lấy ngữ cảnh liên quan từ tài liệu với độ đa dạng cao hơn để sử dụng cho prompt
        """
        try:
            # Lấy nhiều tài liệu hơn so với yêu cầu
            k_expanded = k + 3
            retriever = self.create_retriever(collection_name)
            if not retriever:
                return ""
            
            # Tìm kiếm tài liệu liên quan
            retrieval_results = retriever.get_relevant_documents(query)
            if not retrieval_results:
                logger.warning(f"Không tìm thấy tài liệu nào phù hợp với query: {query}")
                return ""
            
            # Chọn ngẫu nhiên một số tài liệu để tăng tính đa dạng
            if len(retrieval_results) > k:
                # Luôn giữ lại tài liệu có độ liên quan cao nhất
                selected_docs = [retrieval_results[0]]
                
                # Chọn ngẫu nhiên từ các tài liệu còn lại
                random_selection = random.sample(retrieval_results[1:], min(k-1, len(retrieval_results)-1))
                selected_docs.extend(random_selection)
                
                # Sắp xếp lại thứ tự một cách ngẫu nhiên
                if random.random() > 0.7:  # 30% trường hợp đảo thứ tự
                    random.shuffle(selected_docs)
            else:
                selected_docs = retrieval_results
            
            # Thêm seed ngẫu nhiên vào nội dung
            random_seed = random.randint(1000, 9999)
            
            # Tạo ngữ cảnh với các đoạn được lựa chọn
            context_parts = [doc.page_content for doc in selected_docs]
            
            # Thêm chút biến đổi nhỏ cho ngữ cảnh
            final_context = "\n\n".join(context_parts)
            
            # Ghi log thông tin để theo dõi
            logger.info(f"Đã tạo ngữ cảnh đa dạng với {len(selected_docs)} tài liệu liên quan")
            
            return final_context
        except Exception as e:
            logger.error(f"Lỗi khi lấy ngữ cảnh liên quan: {e}")
            return ""

    def generate_questions(self, 
                         query: Dict[str, Any], 
                         collection_name: str = "default",
                         model: str = "deepseek-chat", 
                         difficulty_mode: str = 'random',
                         level_id: int = 2,
                         sample_questions: Optional[List[Dict[str, Any]]] = None) -> List[Dict[str, Any]]:
        """
        Sinh câu hỏi trắc nghiệm dựa trên query và ngữ cảnh từ vector DB
        """
        try:
            # Lấy thông tin từ query
            question_text = query.get('question', '')
            num_questions = query.get('num_questions', 3)
            subject = query.get("subject", question_text)
            
            # Tạo random seed cho mỗi lần sinh câu hỏi
            random_seed = random.randint(1000, 9999)
            
            # Chọn phong cách câu hỏi ngẫu nhiên
            question_style = self.get_question_style()
            
            # Lấy ngữ cảnh liên quan từ vector DB với độ đa dạng cao
            context = vector_db.get_relevant_context(question_text, collection_name)
            
            # Biến đổi ngữ cảnh để tăng tính đa dạng
            if context:
                # Thêm yếu tố ngẫu nhiên vào context
                context_parts = context.split("\n\n")
                if len(context_parts) > 1 and random.random() > 0.5:
                    # Xáo trộn thứ tự các đoạn văn (giữ đoạn đầu tiên)
                    first_part = context_parts[0]
                    remaining_parts = context_parts[1:]
                    random.shuffle(remaining_parts)
                    context_parts = [first_part] + remaining_parts
                    context = "\n\n".join(context_parts)
                
                # Thêm query vào đầu ngữ cảnh
                context = f"Query: {question_text}\n\nContext từ tài liệu:\n{context}"

                #logger.info(f"Đã lấy được ngữ cảnh từ vector DB ({len(context)} ký tự)")
            else:
                logger.warning("Không có ngữ cảnh từ vector DB, sẽ sử dụng chỉ query")
                context = question_text
                print("===== CONTEXT TRUYỀN VÀO PROMPT =====")
                print(context)
                print("=====================================")

            
            # Tạo LLM với temperature có thể thay đổi
            llm = self.create_llm(model, temperature=random.uniform(0.6, 0.9))
            
            # Chuẩn bị prompt dựa trên chế độ độ khó
            if difficulty_mode == 'random':
                prompt = self.prompt_random.format(
                    context=context,
                    num_questions=num_questions,
                    level_id=level_id,
                    random_seed=random_seed,
                    question_style=question_style
                )
            elif difficulty_mode == 'similar':
                # Lấy thông tin kết quả trước đó nếu có
                results = query.get('results', {})
                num_correct = results.get('num_correct', 0)
                total_questions = results.get('total_questions', 0)
                time_taken = results.get('time_taken', 0)
                
                ratio_correct = (num_correct / total_questions * 100) if total_questions > 0 else 0
                avg_time_per_question = time_taken / total_questions if total_questions > 0 else 0
                previous_level = results.get('previous_level', level_id)
                new_level = level_id
                
                # Tạo mô tả đặc điểm học sinh
                student_characteristic = self.get_student_characteristic(results)

                prompt = self.prompt_similar_level.format(
                    context=context,
                    num_questions=num_questions,
                    question=question_text,
                    num_correct=num_correct,
                    subject=subject,
                    total_questions=total_questions,
                    ratio_correct=ratio_correct,
                    avg_time_per_question=avg_time_per_question,
                    previous_level=previous_level,
                    new_level=new_level,
                    random_seed=random_seed,
                    student_characteristic=student_characteristic,
                    question_style=question_style
                )
            else:
                raise ValueError("Chế độ không hợp lệ. Chọn 'random' hoặc 'similar'.")

            # Gọi LLM để sinh câu hỏi
            try:
                response = llm.generate([prompt])
                result_text = response.generations[0][0].text.strip()
            except Exception as e:
                logger.warning(f"Lỗi khi gọi LLM chính: {e}")
                logger.info("Chuyển sang dùng ChatGPT làm fallback...")
                fallback_llm = ChatOpenAI(
                    model="gpt-4",
                    openai_api_key=self.openai_api_key,
                    max_tokens=1000,
                    temperature=0.8  # Tăng nhiệt độ để đa dạng hơn
                )
                response = fallback_llm.generate([prompt])
                result_text = response.generations[0][0].text.strip()

            # Phân tích kết quả
            questions = self._parse_multiple_questions(result_text)
            
            # Loại bỏ các câu hỏi trùng lặp với mẫu (nếu có)
            if sample_questions:
                filtered_questions = [q for q in questions if not self._check_question_similarity(q, sample_questions)]
                return filtered_questions
            
            return questions

        except Exception as e:
            logger.exception(f"Lỗi khi sinh câu hỏi: {e}")
            return []
    
    # def generate_questions_rag(self, query: Dict[str, Any], collection_name: str = "default", model: str = "gpt-4") -> List[Dict[str, Any]]:
    #     """
    #     Sinh câu hỏi sử dụng RAG (Retrieval-Augmented Generation)
    #     """
    #     try:
    #         # Lấy thông tin từ query
    #         question_text = query.get('question', '')
    #         num_questions = query.get('num_questions', 3)
            
    #         # Thêm yếu tố ngẫu nhiên
    #         random_seed = random.randint(1000, 9999)
    #         question_style = self.get_question_style()
            
    #         # Tạo retriever
    #         retriever = self.create_retriever(collection_name)
    #         if not retriever:
    #             logger.warning("Không thể tạo retriever, sẽ sử dụng chế độ không RAG")
    #             return self.generate_questions(query, collection_name, model)
            
    #         # Tạo LLM với temperature cao hơn
    #         llm = self.create_llm(model, temperature=random.uniform(0.7, 0.9))
            
    #         # Tạo RetrievalQA chain
    #         qa_chain = RetrievalQA.from_chain_type(
    #             llm=llm,
    #             retriever=retriever,
    #             chain_type="stuff",
    #             return_source_documents=True
    #         )
            
    #         # Chuẩn bị prompt cho RAG với yếu tố ngẫu nhiên
    #         rag_prompt = f"""
    #         Dựa trên tài liệu đã thu thập, hãy tạo {num_questions} câu hỏi trắc nghiệm về chủ đề: {question_text}.
            
    #         Seed ngẫu nhiên: {random_seed}
            
    #         Hãy sử dụng phong cách câu hỏi: {question_style}
            
    #         Mỗi câu hỏi phải có 4 đáp án và phải trả về định dạng JSON như sau:
    #         [
    #             {{
    #                 "questionText": "Nội dung câu hỏi", 
    #                 "levelId": 2, 
    #                 "answerQuestions": [
    #                     {{"answerText": "Đáp án A", "number": 1}}, 
    #                     {{"answerText": "Đáp án B", "number": 2}},
    #                     {{"answerText": "Đáp án C", "number": 3}}, 
    #                     {{"answerText": "Đáp án D", "number": 4}}
    #                 ],
    #                 "correctAnswer": số từ 1 đến 4 chỉ đáp án đúng
    #             }}
    #         ]
    #         """
            
    #         # Gọi RAG chain
    #         response = qa_chain({"query": rag_prompt})
    #         result_text = response.get("result", "")
            
    #         # Phân tích kết quả
    #         questions = self._parse_multiple_questions(result_text)
    #         return questions
            
    #     except Exception as e:
    #         logger.exception(f"Lỗi khi sử dụng RAG để sinh câu hỏi: {e}")
    #         logger.info("Thử lại với phương pháp thông thường...")
    #         return self.generate_questions(query, collection_name, model)
    
    def generate_large_batch(self, 
                          query: Dict[str, Any], 
                          collection_name: str = "default",
                          model: str = "gpt-4",
                          difficulty_mode: str = 'random', 
                          level_id: int = 2) -> List[Dict[str, Any]]:
        """
        Sinh số lượng lớn câu hỏi bằng cách chia thành các batch nhỏ
        """
        num_questions = query.get('num_questions', 3)
        batch_size = 20  # Tối đa 20 câu/lần
        total_batches = (num_questions + batch_size - 1) // batch_size
        all_questions = []

        for i in range(total_batches):
            batch_questions = min(batch_size, num_questions - len(all_questions))
            logger.info(f"Đang sinh batch {i+1}/{total_batches} ({batch_questions} câu hỏi)...")
            
            # Tạo query cho batch này
            batch_query = query.copy()
            batch_query['num_questions'] = batch_questions
            
            # Thêm biến động cho mỗi batch
            batch_query['batch_id'] = i + 1
            
            response = self.generate_questions(
                query=batch_query,
                collection_name=collection_name,
                model=model,
                difficulty_mode=difficulty_mode,
                level_id=level_id
            )
            
            all_questions.extend(response)
            
            if len(all_questions) >= num_questions:
                break

        return all_questions[:num_questions]
    
    def _check_question_similarity(self, new_question: Dict[str, Any], sample_questions: List[Dict[str, Any]]) -> bool:
        """
        Kiểm tra xem câu hỏi mới có tương tự với các câu hỏi mẫu không
        """
        for sample in sample_questions:
            if new_question["questionText"] == sample["questionText"]:
                return True
        return False
    

    
    def _parse_multiple_questions(self, result_text: str) -> List[Dict[str, Any]]:
        """
        Phân tích và tách nhiều câu hỏi từ chuỗi trả về của LLM
        
        Args:
            result_text: Chuỗi văn bản từ LLM
            
        Returns:
            questions: Danh sách các câu hỏi (dạng dict)
        """
        questions = []
        try:
            # Tìm kiếm mảng JSON hoàn chỉnh
            json_array_pattern = r'\[\s*\{.*?\}\s*\]'
            json_array_match = re.search(json_array_pattern, result_text, re.DOTALL)
            
            if json_array_match:
                # Nếu tìm thấy mảng JSON hoàn chỉnh, sử dụng nó
                try:
                    questions = json.loads(json_array_match.group(0))
                    return questions
                except json.JSONDecodeError:
                    logger.warning("Không thể phân tích mảng JSON hoàn chỉnh, thử phương pháp phân tích từng câu hỏi")
                    pass
                    
            # Tìm tất cả các dấu { để bắt đầu câu hỏi
            start_indices = [match.start() for match in re.finditer(r'\{\s*"questionText"', result_text)]
            
            for i, start_idx in enumerate(start_indices):
                try:
                    # Xác định kết thúc của câu hỏi hiện tại
                    if i < len(start_indices) - 1:
                        end_idx = start_indices[i + 1]
                        question_text = result_text[start_idx:end_idx].strip()
                        # Đảm bảo kết thúc đúng với }
                        if not question_text.endswith('}'):
                            question_text = question_text.rsplit('}', 1)[0] + '}'
                    else:
                        # Câu hỏi cuối cùng
                        question_text = result_text[start_idx:].strip()
                        # Tìm dấu } cuối cùng để đóng đối tượng JSON
                        last_brace = question_text.rfind('}')
                        if last_brace != -1:
                            question_text = question_text[:last_brace + 1]
                    
                    # Sửa lỗi cú pháp phổ biến trong answerQuestions array
                    question_text = re.sub(r'(\}\s*\{)', '},{', question_text)
                    
                    # Sửa lỗi cú pháp thiếu dấu phẩy trong answerQuestions array
                    question_text = re.sub(r'(\}\s*)"answerText"', '},{"answerText"', question_text)
                    
                    # Cố gắng phân tích JSON
                    question = json.loads(question_text)
                    questions.append(question)
                except json.JSONDecodeError as e:
                    logger.error(f"Lỗi khi phân tích câu hỏi JSON: {e}. Dữ liệu: {question_text}")
            
            return questions

        except Exception as e:
            logger.error(f"Lỗi phân tích các câu hỏi: {e}")
            return []
        
