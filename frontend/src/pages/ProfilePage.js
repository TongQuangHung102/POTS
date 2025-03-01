import { useState, useEffect } from "react";
import { Container, Form, Button } from "react-bootstrap";

export default function ProfilePage() {
    const [user, setUser] = useState(null);
    const [formData, setFormData] = useState({ name: "", email: "", role: "" });
    const userId = 1; // Thay bằng userId thực tế

    useEffect(() => {
        fetch(`https://localhost:7259/api/User/${userId}`)
            .then((res) => res.json())
            .then((data) => {
                setUser(data);
                setFormData({ name: data.name, email: data.email, role: data.role });
            })
            .catch((error) => console.error("Error fetching user data:", error));
    }, [userId]);

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        fetch(`https://localhost:7259/api/User/${userId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(formData),
        })
            .then((res) => res.json())
            .then((data) => {
                alert("Profile updated successfully!");
                setUser(data);
            })
            .catch((error) => console.error("Error updating profile:", error));
    };

    return (
        <Container className="p-4" style={{ backgroundColor: "#1A2D42", color: "#D4D8DD", borderRadius: "8px" }}>
            <h2 className="mb-4" style={{ color: "#AAB7B7" }}>User Profile</h2>
            {user ? (
                <Form onSubmit={handleSubmit}>
                    <Form.Group className="mb-3">
                        <Form.Label style={{ color: "#C0C8CA" }}>Name:</Form.Label>
                        <Form.Control
                            type="text"
                            name="name"
                            value={formData.name}
                            onChange={handleChange}
                            style={{ backgroundColor: "#2E4156", color: "#D4D8DD" }}
                        />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label style={{ color: "#C0C8CA" }}>Email:</Form.Label>
                        <Form.Control
                            type="email"
                            name="email"
                            value={formData.email}
                            onChange={handleChange}
                            style={{ backgroundColor: "#2E4156", color: "#D4D8DD" }}
                        />
                    </Form.Group>
                    <Button type="submit" style={{ backgroundColor: "#AAB7B7", color: "#1A2D42", border: "none" }}>
                        Update Profile
                    </Button>
                </Form>
            ) : (
                <p>Loading user data...</p>
            )}
        </Container>
    );
}
