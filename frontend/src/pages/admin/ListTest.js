import React, { useState, useEffect } from "react";
import { Link, useParams } from "react-router-dom";
import './ListChapter.css';


const ListTest = () => {
    const { gradeId } = useParams();
    const [tests, setTests] = useState([]);
    const [newTest, setNewTest] = useState('');
    const [showAdd, setShowAdd] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');
    const [successMessage, setSuccessMessage] = useState('');
    const [isEditing, setIsEditing] = useState(false);
    const [selectedTest, setSelectedTest] = useState(null);
    const [testCategory, setTestCategory] = useState([]);
    const [test, setTest] = useState({
        testId: 0,
        testName: "",
        description: "",
        durationInMinutes: 0,
        maxScore: 0,
        createdAt: Date.UTC,
        isVisible: true,
        order: 10,
        gradeId: 1
    });

    //get du lieu
    useEffect(() => {
        const fetchTest = async () => {
            try {
                const response = await fetch('https://localhost:7259/api/Test/get-all-test');
                const data = await response.json();
                setTests(data);
            } catch (error) {
                console.error("Có lỗi khi lấy dữ liệu bài kiểm tra", error);
            }
            //lay test category
            try {
                const response = await fetch('https://localhost:7259/api/TestCategory/get-all-test-category');
                const data = await response.json();
                setTestCategory(data);
            } catch (error) {
                console.error("Có lỗi khi lấy dữ liệu lớp", error);
            }
        };

        fetchTest();
    }, []);

    //add new
    const handleAddTest = async () => {
        try {
            const response = await fetch('https://localhost:7259/api/Test/add-new-test', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(test),
            });

            if (!response.ok) {
                const errorResponse = await response.json();
                const errorMessage = errorResponse ? errorResponse : 'Không thể thêm bài kiểm tra mới';
                setTimeout(() => {
                    setErrorMessage('');
                }, 3000);
                throw new Error(errorMessage);
            }

            const testResponse = await fetch('https://localhost:7259/api/Test/get-all-test', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            const testData = await testResponse.json();
            setTests(testData);
            setNewTest('');
            setShowAdd(false);
            setErrorMessage('');



            setTimeout(() => {
                setSuccessMessage('');
            }, 3000);
        } catch (error) {
            setErrorMessage(error.message);
            console.error("Có lỗi khi thêm mới", error);
        }
    };

    const handleSelectChange = (e) => {
        setTest({ ...test, testName: e.target.value });
    };

    //edit chapter
    const handleEdit = (t) => {
        setSelectedTest(t);
        setIsEditing(true);
    };

    const handleClose = () => {
        setShowAdd(false);
        setIsEditing(false);
        setSelectedTest(null);
    };

    const handleSave = async () => {
        try {
            const response = await fetch(`https://localhost:7259/api/Test/edit-test/${selectedTest.testId}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    testId: selectedTest.testId,
                    testName: selectedTest.testName,
                    description: selectedTest.description,
                    durationInMinutes: selectedTest.durationInMinutes,
                    maxScore: selectedTest.maxScore,
                    createdAt: selectedTest.createdAt,
                    isVisible: selectedTest.isVisible,
                    order: selectedTest.order,
                    gradeId: 1
                }),
            });
    
            const message = await response.json();
    
            if (!response.ok) {
                throw new Error(message.message || message.title);
            }
    
            alert("Cập nhật thành công!");
            setIsEditing(false);
            setErrorMessage("");
    
            const testResponse = await fetch('https://localhost:7259/api/Test/get-all-test', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
    
            const testData = await testResponse.json();
            setTests(testData);
        } catch (error) {
            setErrorMessage(error.message || error.title);
        }
    };
    



return (
    <div className="chapter-list-container">
        <h2>Danh Sách Bài Kiểm Tra</h2>
        <div className="group-header">
            <div>
                <Link className="backlink" to='/admin'>Trang chủ</Link>/ Loại bài
            </div>
            <button className="add-chapter" onClick={() => setShowAdd(true)}>Thêm mới</button>
        </div>
        {showAdd && (
            <div className="modal">
                <div className="modal-content">
                    <h3>Thêm mới bài test</h3>
                    <label>
                        Tên bài kiểm tra:
                        <select name="testName" value={test.categoryName} onChange={handleSelectChange} required>
                            {testCategory.map((item) => (
                                <option key={item.testCategoryId} value={item.categoryName}>
                                    {item.categoryName}
                                </option>
                            ))}
                        </select>
                    </label>
                    <label>
                        Thời gian làm bài:
                        <input type="number" name="durationInMinutes" value={test.durationInMinutes} onChange={(e) => setTest({ ...test, durationInMinutes: e.target.value })} placeholder="Thời gian (phút)" required />
                    </label>
                    <label>
                        Điểm tối đa:
                        <input type="number" name="maxScore" value={test.maxScore} onChange={(e) => setTest({ ...test, maxScore: e.target.value })} placeholder="Điểm tối đa" required />
                    </label>
                    <label>
                        Trạng thái:
                        <select name="isVisible" value={test.isVisible} onChange={(e) => setTest({ ...test, isVisible: e.target.value === "true" })}>
                            <option value="true">Hiển thị</option>
                            <option value="false">Ẩn</option>
                        </select>
                    </label>
                    <div className="button-group">
                        <button onClick={handleAddTest}>Thêm</button>
                        <button onClick={handleClose}>Đóng</button>
                    </div>
                    {errorMessage && <p className="error-message">{errorMessage}</p>}
                </div>
            </div>

        )}
        {errorMessage && <div className="error-message">{errorMessage}</div>}
        {successMessage && <div className="success-message">{successMessage}</div>}
        <table className="chapter-table">
            <thead>
                <tr>
                    <th style={{ width: "10%" }}>Id</th>
                    <th style={{ width: "30%" }}>Tên</th>
                    <th style={{ width: "20%" }}>Thời gian làm bài</th>
                    <th style={{ width: "15%" }}>Điểm tối đa</th>
                    <th style={{ width: "15%" }}>Trạng thái</th>
                    <th>Hành động</th>
                </tr>
            </thead>
            <tbody>
                {tests.map((t) => (
                    <tr key={t.testId}>
                        <td>{t.testId}</td>
                        <td>{t.testName}</td>
                        <td>{t.durationInMinutes}</td>
                        <td>{t.maxScore}</td>
                        <td>{t.isVisible ? <span style={{ color: "green" }}>Hoạt động</span> : <span style={{ color: "red" }}>Không hoạt động</span>}</td>
                        <td>
                            <button onClick={() => handleEdit(t)}>Chỉnh sửa</button>
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
        {/* Form chỉnh sửa */}
        {isEditing && (
                <div className="modal">
                    <div className="modal-content">
                        <h3>Chỉnh sửa loại bài</h3>
                        <label>
                            Tên:
                            <input
                                type="text"
                                value={selectedTest?.testName}
                                onChange={(e) =>
                                    setSelectedTest({ ...selectedTest, testName: e.target.value })
                                }
                                disabled
                            />
                        </label>
                        <label>
                            Mô tả:
                            <input
                                type="text"
                                value={selectedTest?.description}
                                onChange={(e) =>
                                    setSelectedTest({ ...selectedTest, description: e.target.value })
                                }
                            />
                        </label>
                        <label>
                            Thời gian làm bài:
                            <input
                                type="text"
                                value={selectedTest?.durationInMinutes}
                                onChange={(e) =>
                                    setSelectedTest({ ...selectedTest, durationInMinutes: e.target.value })
                                }
                            />
                        </label>
                        <label>
                            Điểm tối đa:
                            <input
                                type="text"
                                value={selectedTest?.maxScore}
                                onChange={(e) =>
                                    setSelectedTest({ ...selectedTest, maxScore: e.target.value })
                                }
                            />
                        </label>
                        <label>
                            Trạng thái:
                            <select
                                value={selectedTest?.isVisible}
                                onChange={(e) =>
                                    setSelectedTest({
                                        ...selectedTest,
                                        isVisible: e.target.value === "true"
                                    })
                                }
                            >
                                <option value="true">Hoạt động</option>
                                <option value="false">Không hoạt động</option>
                            </select>
                        </label>
                        <div className="button-group">
                            <button onClick={handleSave}>Lưu</button>
                            <button onClick={handleClose}>Đóng</button>
                        </div>
                        {errorMessage && <p className="error-message">{errorMessage}</p>}
                    </div>
                </div>
            )}
        </div>
    );
};

export default ListTest;
