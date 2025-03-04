from flask import Flask, request, jsonify
from openai_query import ask_openai, generate_large_mcq

app = Flask(__name__)

@app.route('/generate-mcq', methods=['POST'])
def generate_mcq():
    """API để tạo câu hỏi trắc nghiệm từ OpenAI."""
    data = request.get_json() if request.is_json else request.form
    user_question = data.get('question')  
    num_questions = int(data.get('num_questions', 5))

    # Kiểm tra xem câu hỏi và số lượng câu hỏi có hợp lệ không
    if not user_question:
        return jsonify({"error": "No question provided"}), 400

    if not num_questions or not isinstance(num_questions, int) or num_questions <= 0:
        return jsonify({"error": "Invalid number of questions"}), 400

    # Nếu số lượng câu hỏi > 20, chia nhỏ truy vấn
    if num_questions > 20:
        questions = generate_large_mcq(user_question, num_questions)
    else:
        questions = ask_openai(user_question, num_questions)

    return jsonify({"questions": questions})


@app.route('/')
def home():
    return "Welcome to the Question Generator API!"

if __name__ == '__main__':
    app.run(debug=True)
