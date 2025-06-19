import { useEffect, useState, useRef } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { authFetch } from '../utils/authFetch';

const ConfirmEmailPage = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [message, setMessage] = useState('');
    const apiUrl = process.env.REACT_APP_API_URL;
    const calledOnce = useRef(false);

    useEffect(() => {
        if (calledOnce.current) return;
        calledOnce.current = true;

        const searchParams = new URLSearchParams(location.search);
        const token = searchParams.get('token'); 

        const confirmEmail = async () => {
            try {
                const response = await authFetch(`${apiUrl}/api/Auth/Confirm-email?token=${token}`);
                const redirectUrl = await response.text();
                
                if (redirectUrl) {
                    window.location.href = redirectUrl;
                } else {
                    setMessage('Xác nhận email thành công!');
                }
            } catch (error) {
                console.error(error);
                setMessage('Xác nhận email thất bại. Vui lòng thử lại.');
            } finally {
                setLoading(false);
            }
        };

        confirmEmail();
    }, [location.search, apiUrl]);

    return (
        <div className="confirm-email-page">
            <h1>Xác nhận Email</h1>
            {loading ? (
                <p>Đang xác nhận...</p>
            ) : (
                <p>{message || 'Đang chuyển hướng...'}</p>
            )}
        </div>
    );
};

export default ConfirmEmailPage;