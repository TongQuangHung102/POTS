# from langchain.prompts import PromptTemplate
# from langchain_openai import OpenAI

# # Đặt API key của OpenAI
# openai_api_key = "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"

# # Adjust the max_tokens to a smaller value to leave room for the prompt
# # The total of prompt tokens + max_tokens must be <= 4097
# llm = OpenAI(api_key=openai_api_key, max_tokens=3500)

# # Tạo prompt mẫu
# prompt = PromptTemplate.from_template("""
# Context: {context}

# Hãy tạo {num_questions} câu hỏi trắc nghiệm về chủ đề này với 4 đáp án (A, B, C, D) cho mỗi câu hỏi. 
# Mỗi câu hỏi bao gồm các trường sau:
# 1. Nội dung câu hỏi.
# 2. Mức độ câu hỏi (levelId) từ 1 đến 4.
# 3. 4 đáp án với nội dung và số thứ tự từ 1 đến 4 (A, B, C, D).
# 4. Đáp án đúng được chỉ định bằng số thứ tự của đáp án (1, 2, 3, 4).
# Mỗi câu hỏi cần trả về cấu trúc dưới đây:

# {{
#     "questionText": "Nội dung câu hỏi",
#     "levelId": 1,
#     "answerQuestions": [
#         {{"answerText": "Đáp án A", "number": 1}},
#         {{"answerText": "Đáp án B", "number": 2}},
#         {{"answerText": "Đáp án C", "number": 3}},
#         {{"answerText": "Đáp án D", "number": 4}}
#     ],
#     "correctAnswer": 1
# }}
# """)

# def ask_openai(context, num_questions):
#     """Hỏi OpenAI để tạo câu hỏi trắc nghiệm"""
#     questions = []
    
#     # Adjust number of questions based on context length
#     # For longer contexts, generate fewer questions per request
#     context_length = len(context)
#     batch_size = min(num_questions, max(1, 10 - (context_length // 500)))
    
#     for i in range(0, num_questions, batch_size):
#         current_batch = min(batch_size, num_questions - i)
        
#         # Tạo prompt với các tham số động
#         formatted_prompt = prompt.format(
#             context=context,
#             num_questions=current_batch
#         )
        
#         # Gọi API của OpenAI để tạo câu hỏi trắc nghiệm
#         response = llm.generate([formatted_prompt])
        
#         # Lưu kết quả vào danh sách questions
#         questions.append(response.generations[0][0].text.strip())
    
#     return questions

from langchain.prompts import PromptTemplate
from langchain_openai import OpenAI
import json

# Đặt API key của OpenAI
openai_api_key = "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"

# Sử dụng max_tokens nhỏ hơn để đảm bảo không vượt quá giới hạn
llm = OpenAI(api_key=openai_api_key, max_tokens=2500, temperature=0.2)

# Tạo prompt mẫu với sự nhấn mạnh vào định dạng chuẩn
prompt = PromptTemplate.from_template("""
Context: {context}

Hãy tạo CHÍNH XÁC {num_questions} câu hỏi trắc nghiệm về chủ đề ở trên với 4 đáp án (A, B, C, D) cho mỗi câu hỏi.

CHÚ Ý: Định dạng phải CHÍNH XÁC theo cấu trúc JSON và phải LUÔN trả về mảng các câu hỏi.

Mỗi câu hỏi PHẢI được định dạng CHÍNH XÁC như sau:
{{
    "questionText": "Nội dung câu hỏi",
    "levelId": số từ 1 đến 4,
    "answerQuestions": [
        {{"answerText": "Đáp án A", "number": 1}},
        {{"answerText": "Đáp án B", "number": 2}},
        {{"answerText": "Đáp án C", "number": 3}},
        {{"answerText": "Đáp án D", "number": 4}}
    ],
    "correctAnswer": số từ 1 đến 4 chỉ đáp án đúng
}}

TRẢ VỀ mảng JSON của {num_questions} câu hỏi, định dạng chính xác như sau:
[
  {{câu hỏi 1}},
  {{câu hỏi 2}},
  ...
]

KHÔNG ĐƯỢC thêm bất kỳ văn bản giải thích nào ngoài mảng JSON. KHÔNG ĐƯỢC thêm bất kỳ thông tin nào về bối cảnh. CHỈ trả về mảng JSON các câu hỏi theo định dạng yêu cầu.
""")

def ask_openai(context, num_questions):
    """Hỏi OpenAI để tạo câu hỏi trắc nghiệm với kiểm soát định dạng tốt hơn"""
    # Tính số câu hỏi tối đa cho mỗi lần gọi API dựa trên độ dài context
    context_length = len(context)
    max_questions_per_batch = max(1, min(5, 10 - (context_length // 300)))
    
    all_questions = []
    
    for i in range(0, num_questions, max_questions_per_batch):
        current_batch = min(max_questions_per_batch, num_questions - i)
        
        # Tạo prompt với số lượng câu hỏi hiện tại
        formatted_prompt = prompt.format(
            context=context,
            num_questions=current_batch
        )
        
        try:
            # Gọi API của OpenAI
            response = llm.generate([formatted_prompt])
            result_text = response.generations[0][0].text.strip()
            
            # Cố gắng phân tích kết quả thành JSON
            # Loại bỏ các ký tự không mong muốn có thể xuất hiện trước hoặc sau JSON
            result_text = result_text.strip()
            
            # Tìm dấu ngoặc vuông đầu tiên và cuối cùng
            start_index = result_text.find('[')
            end_index = result_text.rfind(']') + 1
            
            if start_index >= 0 and end_index > start_index:
                json_text = result_text[start_index:end_index]
                # Phân tích cú pháp JSON
                parsed_questions = json.loads(json_text)
                
                # Kiểm tra cấu trúc của mỗi câu hỏi
                valid_questions = []
                for q in parsed_questions:
                    if (isinstance(q, dict) and 
                        "questionText" in q and 
                        "levelId" in q and 
                        "answerQuestions" in q and 
                        "correctAnswer" in q and
                        len(q["answerQuestions"]) == 4):
                        valid_questions.append(q)
                
                all_questions.extend(valid_questions)
            else:
                # Nếu không tìm thấy cặp ngoặc vuông hợp lệ, thử xử lý như đối tượng JSON đơn
                result_text = result_text.strip()
                start_index = result_text.find('{')
                end_index = result_text.rfind('}') + 1
                
                if start_index >= 0 and end_index > start_index:
                    json_text = result_text[start_index:end_index]
                    try:
                        single_question = json.loads(json_text)
                        if (isinstance(single_question, dict) and 
                            "questionText" in single_question and 
                            "levelId" in single_question and 
                            "answerQuestions" in single_question and 
                            "correctAnswer" in single_question):
                            all_questions.append(single_question)
                    except json.JSONDecodeError:
                        # Nếu không phân tích được JSON, bỏ qua kết quả này
                        pass
        
        except json.JSONDecodeError:
            # Xử lý khi định dạng JSON không hợp lệ
            print(f"Không thể phân tích kết quả thành JSON: {result_text}")
            continue
    
    # Kiểm tra nếu chưa đủ số câu hỏi, tạo thêm
    if len(all_questions) < num_questions:
        remaining = num_questions - len(all_questions)
        print(f"Cần tạo thêm {remaining} câu hỏi")
        # Gọi đệ quy nhưng với số lượng câu hỏi còn thiếu
        additional_questions = ask_openai(context, remaining)
        all_questions.extend(additional_questions)
    
    # Giới hạn số câu hỏi đúng theo yêu cầu
    return all_questions[:num_questions]

def generate_large_mcq(user_question, num_questions):
    """Chia nhỏ truy vấn nếu số lượng câu hỏi lớn"""
    batch_size = 20  # Tạo tối đa 20 câu/lần
    total_batches = (num_questions + batch_size - 1) // batch_size
    all_questions = []

    for i in range(total_batches):
        batch_questions = min(batch_size, num_questions - len(all_questions))
        response = ask_openai(user_question, batch_questions)
        all_questions.extend(response)  # Thêm vào danh sách kết quả

    return all_questions