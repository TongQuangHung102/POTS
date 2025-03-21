{/* Form chỉnh sửa */}
            {isEditing && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <h3>Chỉnh sửa câu hỏi</h3>
                        <label>
                            Câu hỏi:
                            <input
                                type="text"
                                value={editingQuestion.question}
                                required
                                onChange={(e) =>
                                    setEditingQuestion({ ...editingQuestion, question: e.target.value })
                                }

                            />
                        </label>
                        <label>Trạng thái hiển thị:
                            <select
                                value={editingQuestion.isVisible}
                                onChange={(e) =>
                                    setEditingQuestion({ ...editingQuestion, isVisible: e.target.value })
                                }
                            >
                                <option value="true">Hiển thị</option>
                                <option value="false">Ẩn</option>
                            </select>
                        </label>
                        <label>Mức độ:
                            <select
                                value={editingQuestion.levelId}
                                onChange={(e) =>
                                    setEditingQuestion({ ...editingQuestion, levelId: e.target.value })
                                }
                            >
                                {levels?.map((l) => (
                                    <option key={l.levelId} value={l.levelId}>
                                        {l.levelName}
                                    </option>
                                ))}
                            </select>
                        </label>


                        <label>Các câu trả lời:
                            {editingQuestion.options.map((option, index) => (
                                <div key={option.id} className={styles.answer} >
                                    <input
                                        className={`${styles.answerItem} ${option.id === editingQuestion.correctAnswer ? styles.correctAnswer : ""
                                            }`}
                                        type="text"
                                        value={option.text}
                                        required
                                        onChange={(e) => {
                                            const updatedOptions = [...editingQuestion.options];
                                            updatedOptions[index].text = e.target.value;
                                            setEditingQuestion({ ...editingQuestion, options: updatedOptions });
                                        }}

                                    />
                                    <input className={styles.radioInput}
                                        type="radio"
                                        name="correctAnswer"
                                        value={option.id}
                                        checked={editingQuestion.correctAnswer === option.id}
                                        onChange={(e) =>
                                            setEditingQuestion({ ...editingQuestion, correctAnswer: Number(e.target.value) })
                                        }
                                    /> Đáp án đúng
                                </div>
                            ))}
                        </label>

                        <div className="button-group">
                            <button onClick={handleSave}>Lưu</button>
                            <button onClick={handleClose}>Đóng</button>
                        </div>
                        {errorMessage && <p className="error-message">{errorMessage}</p>}
                    </div>
                </div>
            )}