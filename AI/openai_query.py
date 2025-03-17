# openai_query.py

import json
import re
import logging
from langchain.prompts import PromptTemplate
from langchain_openai import OpenAI

# Đặt API key của OpenAI
openai_api_key = "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"

# Cấu hình logging để ghi lại các lỗi
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

def create_llm_with_prompt(max_tokens=1000, temperature=0.2):
    """Tạo LLM với cấu hình tùy chỉnh"""
    return OpenAI(
        api_key=openai_api_key,
        max_tokens=max_tokens,  # Giới hạn token cho mỗi câu hỏi
        temperature=temperature
    )

def check_question_similarity(new_question, sample_questions):
    """
    Kiểm tra sự tương tự giữa câu hỏi mới và các câu hỏi mẫu
    """
    for sample in sample_questions:
        if new_question["questionText"] == sample["questionText"]:
            return True  # Trùng với một trong các câu hỏi mẫu
    return False


def ask_openai(context, num_questions, sample_questions=None, difficulty_mode='random', level_id=2):
    """
    Hỏi OpenAI để tạo câu hỏi trắc nghiệm với chế độ khác nhau và đảm bảo không trùng lặp với câu hỏi mẫu.
    
    difficulty_mode:
    - 'random': Tạo câu hỏi với mức độ khó ngẫu nhiên
    - 'similar': Tạo câu hỏi có mức độ khó tương tự
    """
    # Tạo LLM
    llm = create_llm_with_prompt(max_tokens=1000)  # Giới hạn token cho mỗi câu hỏi

    prompt_random = PromptTemplate.from_template("""
Context: {context}

Hãy tạo CHÍNH XÁC {num_questions} câu hỏi trắc nghiệm với MỨC ĐỘ KHÁC NHAU về chủ đề ở trên với 4 đáp án (A, B, C, D) cho mỗi câu hỏi.

CHÚ Ý:
- Phân bố mức độ câu hỏi từ dễ đến khó (1-4)
- Định dạng JSON PHẢI ĐÚNG CÚ PHÁP không thiếu dấu phẩy, ngoặc
- LUÔN trả về duy nhất một mảng JSON trong dấu ngoặc vuông [ ]
- ĐẢM BẢO TẤT CẢ các phần tử trong mảng answerQuestions đều được phân tách bằng dấu phẩy
- Không thêm bất kỳ văn bản giải thích nào ngoài mảng JSON.

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

TRẢ VỀ mảng JSON của {num_questions} câu hỏi. Mỗi câu hỏi phải là một phần tử trong mảng này.
""")


    # Prompt cho câu hỏi có mức độ tương tự
    prompt_similar_level = PromptTemplate.from_template("""Context: {context}

Dựa trên kết quả sau đây:
- Số câu trả lời đúng: {num_correct}
- Tổng số câu hỏi: {total_questions}
- Thời gian làm bài: {time_taken} giây

Hãy phân tích và xác định MỨC ĐỘ (mức độ khó) của người dùng. Mức độ có thể là:
1. Mức độ dễ (beginner): Người dùng trả lời đúng ít câu và mất nhiều thời gian.
2. Mức độ trung bình (intermediate): Người dùng trả lời đúng một phần câu hỏi và mất thời gian vừa phải.
3. Mức độ khó (advanced): Người dùng trả lời đúng nhiều câu và làm bài nhanh.
4. Mức độ rất khó (expert): Người dùng trả lời đúng tất cả câu hỏi trong thời gian rất ngắn.

Ví dụ:
- Nếu người dùng trả lời đúng tất cả câu hỏi trong vòng 10 giây (hoặc thời gian nhanh tương tự), họ có thể thuộc mức độ "advanced" hoặc "expert".
- Nếu người dùng trả lời đúng ít câu hoặc mất nhiều thời gian, họ có thể thuộc mức độ "beginner" hoặc "intermediate".

Sau khi phân tích mức độ người dùng, hãy tạo {num_questions} câu hỏi trắc nghiệm với mức độ khó tương ứng với mức độ đã phân tích.

Yêu cầu:
- Mức độ câu hỏi: {user_level}
- Thay đổi tham số, tình huống nhưng giữ nguyên độ khó
- 4 đáp án (A, B, C, D) cho mỗi câu hỏi

CHÚ Ý:
- Phân bố mức độ câu hỏi từ dễ đến khó (1-4)
- Định dạng JSON PHẢI ĐÚNG CÚ PHÁP không thiếu dấu phẩy, ngoặc
- LUÔN trả về duy nhất một mảng JSON trong dấu ngoặc vuông [ ]
- ĐẢM BẢO TẤT CẢ các phần tử trong mảng answerQuestions đều được phân tách bằng dấu phẩy
- Không thêm bất kỳ văn bản giải thích nào ngoài mảng JSON.

Định dạng JSON chính xác:
{{
    "questionText": "Nội dung câu hỏi",
    "levelId": {user_level}, 
    "answerQuestions": [
    {{"answerText": "Đáp án A", "number": 1}},
    {{"answerText": "Đáp án B", "number": 2}},
    {{"answerText": "Đáp án C", "number": 3}},
    {{"answerText": "Đáp án D", "number": 4}}],
    "correctAnswer": số từ 1 đến 4 chỉ đáp án đúng
}}

TRẢ VỀ mảng JSON của {num_questions} câu hỏi.
KHÔNG ĐƯỢC thêm bất kỳ văn bản giải thích nào ngoài mảng JSON.
""")


    try:
        # Chọn prompt và định dạng
        if difficulty_mode == 'random':
            # Truyền vào prompt cho câu hỏi ngẫu nhiên
            prompt = prompt_random.format(
                context=context, 
                num_questions=num_questions, 
                level_id=level_id
            )
        elif difficulty_mode == 'similar':
            # Lấy kết quả từ context nếu có
            if isinstance(context, dict):
                context_text = context.get('context', '')
                results = context.get('results', {})  # Lấy kết quả từ context
            else:
                context_text = context
                results = {}

            # Truyền kết quả vào prompt
            num_correct = results.get('num_correct', 0)
            total_questions = results.get('total_questions', 0)
            time_taken = results.get('time_taken', 0)

            prompt = prompt_similar_level.format(
                context=context_text, 
                num_questions=num_questions, 
                num_correct=num_correct, 
                total_questions=total_questions, 
                time_taken=time_taken,
                user_level=level_id  # Có thể là beginner, intermediate, advanced
            )
        else:
            raise ValueError("Chế độ không hợp lệ. Chọn 'random' hoặc 'similar'.")

        # Gọi API của OpenAI
        response = llm.generate([prompt])
        result_text = response.generations[0][0].text.strip()

        #Phân tích JSON từ chuỗi trả về của OpenAI
        questions = parse_multiple_questions(result_text)

        # Kiểm tra trùng lặp câu hỏi với câu hỏi mẫu
        if sample_questions:
            filtered_questions = []
            for question in questions:
                if not check_question_similarity(question, sample_questions):
                    filtered_questions.append(question)

            if filtered_questions:
                return filtered_questions
            else:
                logger.warning("Tất cả các câu hỏi sinh ra đều trùng với câu hỏi mẫu.")
                return []

        return questions

    except Exception as e:
        logger.error(f"Lỗi trong quá trình sinh câu hỏi: {e}")
        return []


def generate_large_mcq(context, num_questions, difficulty_mode='random', level_id=2):
    """
    Hàm sinh câu hỏi với số lượng lớn
    Chia nhỏ truy vấn nếu số lượng câu hỏi lớn
    """
    batch_size = 20  # Tối đa 20 câu/lần
    total_batches = (num_questions + batch_size - 1) // batch_size
    all_questions = []

    for _ in range(total_batches):
        batch_questions = min(batch_size, num_questions - len(all_questions))
        response = ask_openai(context, batch_questions, difficulty_mode, level_id)
        all_questions.extend(response)

    return all_questions[:num_questions]



def parse_multiple_questions(result_text):
    """
    Hàm để phân tích và tách nhiều câu hỏi từ chuỗi trả về của OpenAI, sửa lỗi cú pháp JSON nếu có.
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
                #logger.warning("Không thể phân tích mảng JSON hoàn chỉnh, thử phương pháp phân tích từng câu hỏi")
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
