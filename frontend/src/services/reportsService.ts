import api from "./api"


export const getSpendingByCategory = async () => {
    const response = await api.get(`/Reports/spending-by-category`)
    return response.data;
}

export const getIncomeVsExpenseTrends = async () => {
    const response = await api.get(`/Reports/income-vs-expense-trends`)
    return response.data;
}