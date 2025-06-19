import { authFetch } from "../utils/authFetch";
const apiUrl = process.env.REACT_APP_API_URL; 

export const getSubscriptionPlanById = async (planId) => {
    try {
        const response = await authFetch(
            `${apiUrl}/api/SubscriptionPlan/get-subscriptionplan-by/${planId}`,
            {
                method: 'GET',
            }
        );

        if (!response.ok) {
            throw new Error('Lỗi khi lấy chi tiết gói');
        }

        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Lỗi trong getSubscriptionPlanById:', error);
        throw error;
    }
};
