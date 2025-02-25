import React from 'react';
import { Container, Row, Col, Card, Button } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';

const CoursePricing = () => {
    const plans = [
        { color: 'primary', price: '$29.99', disk: '200GB', bandwidth: '30GB', templates: '2', domains: 'No', email: 'No' },
        { color: 'purple', price: '$39.99', disk: '300GB', bandwidth: '45GB', templates: '5', domains: 'Yes', email: 'Yes' },
        { color: 'warning', price: '$99.99', disk: '500GB', bandwidth: '90GB', templates: '20', domains: 'Yes', email: 'Yes' }
    ];

    return (
        <Container className="d-flex justify-content-center align-items-center vh-100">
            <Row className="g-4">
                {plans.map((plan, index) => (
                    <Col key={index} md={4}>
                        <Card className={`text-white bg-${plan.color} text-center`}>
                            <Card.Body>
                                <Card.Title className="fs-3">{plan.price} Per Month</Card.Title>
                                <Card.Text>
                                    Disk Space: {plan.disk}<br />
                                    Bandwidth: {plan.bandwidth}<br />
                                    Style Templates: {plan.templates}<br />
                                    Domains: {plan.domains}<br />
                                    Email Account: {plan.email}<br />
                                    First 30 Days Free
                                </Card.Text>
                                <Button variant="light">Buy Now</Button>
                            </Card.Body>
                        </Card>
                    </Col>
                ))}
            </Row>
        </Container>
    );
};

export default CoursePricing;