import React from 'react';
import { Container, Row, Col, Card } from 'react-bootstrap';
import LoginForm from './Components/LoginForm';
import RegisterForm from './Components/RegisterForm';
import ForgotPasswordForm from './Components/ForgotPasswordForm';

function App() {
  const [currentForm, setCurrentForm] = React.useState('login');

  return (
    <Container style={{
      minHeight: '100vh',
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      backgroundColor: '#1A2D42'
    }}>
      <Row>
        <Col >
          <Card
            style={{
              backgroundColor: '#2E4156',
              borderRadius: '15px',
              padding: '30px'
            }}
          >
            <Card.Body>
              {currentForm === 'login' && <LoginForm onForgotPassword={() => setCurrentForm('forgotPassword')} onRegister={() => setCurrentForm('register')} />}
              {currentForm === 'register' && <RegisterForm onLogin={() => setCurrentForm('login')} />}
              {currentForm === 'forgotPassword' && <ForgotPasswordForm onLogin={() => setCurrentForm('login')} />}
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
}

export default App;