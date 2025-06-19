import styles from './PaymentInfoPage.module.css';
import { useEffect, useState } from 'react';
import { useLocation, useNavigate  } from 'react-router-dom';
import { authFetch } from '../../utils/authFetch';

const PaymentInfoPage = () => {
  const { state } = useLocation();
  const payment = state?.payment;
  const apiUrl = process.env.REACT_APP_API_URL; 
  const [isChecking, setIsChecking] = useState(false);
  const [canClick, setCanClick] = useState(false);
  const [message, setMessage] = useState('');
  const [hasFailed, setHasFailed] = useState(false);

  const navigate = useNavigate();

  const paymentData = {
    accountName: 'VU DANH HUY',
    accountNumber: '00001628681',
    bank: 'Ngân hàng TMCP Tiên Phong',
    qrImageUrl: `https://qr.sepay.vn/img?acc=00001628681&bank=TPBank&amount=${payment.amount}&des=${payment.transactionId}&template=TEMPLATE&download=DOWNLOAD`,
  };

  useEffect(() => {

    if (!isChecking) return;

    const intervalId = setInterval(async () => {
        const response = await authFetch(`${apiUrl}/api/Payments/status/${payment.paymentId}`);
        const data = await response.json();

        if (data.status === 'Confirmed') {
            clearInterval(intervalId);
            alert('Thanh toán thành công! Cảm ơn bạn');
            navigate('/parent');
        }
        if(data.status === 'Failed'){
          clearInterval(intervalId);
          setHasFailed(true); // đánh dấu đã thất bại
          setIsChecking(false);
          setMessage('');
          alert(`Thất bại, lý do: ${data.reason}! Vui lòng kiểm tra email và làm theo hướng dẫn`);
        }
    }, 5000);

    return () => clearInterval(intervalId);
}, [isChecking]);

useEffect(() => {
    const timer = setTimeout(() => {
        setCanClick(true);
    }, 10000); 

    return () => clearTimeout(timer);
}, []);

const handleCheckPayment = () => {
    setMessage('Chờ chút, chúng tôi đang kiểm tra thanh toán...');
    setIsChecking(true);
    setHasFailed(false); 
};

  return (
    <div className={styles.page}>
      <h1 className={styles.header}>Thông tin chuyển khoản ngân hàng</h1>

      <div className={styles.card}>
        <img src={paymentData.qrImageUrl} alt="QR ngân hàng" className={styles.qr} />

        <p className={styles.alert}>
          Vui lòng quét mã QR để thanh toán hoặc chuyển khoản theo đúng thông tin bên dưới!
        </p>

        <table className={styles.infoTable}>
          <tbody>
            <tr>
              <td>Tên tài khoản:</td>
              <td><strong>{paymentData.accountName}</strong></td>
            </tr>
            <tr>
              <td>Số tài khoản:</td>
              <td><strong>{paymentData.accountNumber}</strong></td>
            </tr>
            <tr>
              <td>Ngân hàng:</td>
              <td><strong>{paymentData.bank}</strong></td>
            </tr>
            <tr>
              <td>Số tiền:</td>
              <td><strong>{payment.amount.toLocaleString('vi-VN')} VND</strong></td>
            </tr>
            <tr>
              <td>Nội dung chuyển khoản:</td>
              <td><strong>{payment.transactionId}</strong></td>
            </tr>
          </tbody>
        </table>

        <button className='btn btn-success mt-4' disabled={!canClick} onClick={handleCheckPayment}>
            {hasFailed ? 'Tôi đã thanh toán lại' : 'Tôi đã thanh toán'}
            </button>
            {message && (
                <p style={{ color: '#007bff', marginTop: '12px' }}>{message}</p>
            )}
      </div>
    </div>
  );
};

export { PaymentInfoPage };
