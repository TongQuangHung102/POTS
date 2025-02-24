// src/components/PricingCards.js
import React from 'react';
import { Container, Row, Col, Button } from 'react-bootstrap';
import './PricingCards.css'; // file CSS riêng (tuỳ chọn)

const PricingCards = () => {
    const pricingData = [
        {
            title: '$29.99',
            subtitle: 'Per Month',
            diskSpace: '200GB',
            bandwidth: '30GB',
            templates: '2',
            domains: 'No',
            email: 'No',
            trial: 'First 30 Days Free',
            gradient: 'linear-gradient(135deg, #1E90FF, #0B60D7)', // ví dụ
            buttonColor: 'primary',
        },
        {
            title: '$39.99',
            subtitle: 'Per Month',
            diskSpace: '300GB',
            bandwidth: '45GB',
            templates: '5',
            domains: 'Yes',
            email: 'Yes',
            trial: 'First 30 Days Free',
            gradient: 'linear-gradient(135deg, #8A2BE2, #663399)',
            buttonColor: 'secondary',
        },
        {
            title: '$99.99',
            subtitle: 'Per Month',
            diskSpace: '500GB',
            bandwidth: '90GB',
            templates: '20',
            domains: 'Yes',
            email: 'Yes',
            trial: 'First 30 Days Free',
            gradient: 'linear-gradient(135deg, #FF8C00, #FF4500)',
            buttonColor: 'warning',
        },
    ];

    return (
        <Container className="py-5">
            <Row className="justify-content-center">
                {pricingData.map((plan, index) => (
                    <Col key={index} xs={12} md={6} lg={4} className="mb-4 d-flex justify-content-center">
                        <div
                            className="pricing-card text-white text-center p-4"
                            style={{
                                background: plan.gradient,
                                borderRadius: '20px',
                                width: '250px',
                            }}
                        >
                            <h3 className="mb-0">{plan.title}</h3>
                            <p className="mb-3">{plan.subtitle}</p>
                            <p>Disk Space: {plan.diskSpace}</p>
                            <p>Bandwidth: {plan.bandwidth}</p>
                            <p>Style Templates: {plan.templates}</p>
                            <p>Domains: {plan.domains}</p>
                            <p>Email Accounts: {plan.email}</p>
                            <p>{plan.trial}</p>

                            <Button variant="light" className="mt-3">
                                Buy Now
                            </Button>
                        </div>
                    </Col>
                ))}
            </Row>
        </Container>
    );
};

export default PricingCards;
