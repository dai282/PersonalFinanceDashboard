import { useState, useEffect } from "react";
import { Category, CreateBudget } from "../../types";
import api from "../../services/api";

interface CreateBudgetFormProps {
  onClose: () => void;
  categories: Category[];
}

export default function CreateBudgetForm({
  onClose, categories
}: CreateBudgetFormProps) {
  const [formData, setFormData] = useState<CreateBudget>({
    categoryId: 0,
    amount: 0,
    month: 0,
    year: 0,
  });

  const [pressedSubmit, setpressedSubmit] = useState(false);
  const [submissionResult, setSubmissionResult] = useState<string>();

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;

    setFormData((prevState) => ({
      ...prevState!,
      [name]: value,
    }));
  };

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setpressedSubmit(true);
    setTimeout(() => {
      setpressedSubmit(false);
    }, 3000);
    if (formData.categoryId <= 0) {
      setSubmissionResult("Please select a category!");
      return;
    }
    if (formData.amount <= 0) {
      setSubmissionResult("Amount must be greater than 0!");
      return;
    }
    if (formData.month < 1 || formData.month > 12) {
      setSubmissionResult("Month must be between 1 and 12!");
      return;
    }
    if (formData.year < 2000 || formData.year > 2100) {
      setSubmissionResult("Year must be between 2000 and 2100!");
      return;
    }
    try {
      setSubmissionResult("Success!");
      const response = await api.post("/budgets", formData);
      console.log(response);
    } catch (error) {
      console.error(error);
      setSubmissionResult((error as any)?.response?.data || "An error occurred");
    }
  }

  return (
    <div className="max-w-md mx-auto mt-10">
      <form
        onSubmit={handleSubmit}
        className="bg-white shadow-md rounded px-8 pt-6 pb-8 mb-4"
      >
        {pressedSubmit && (
          <div
            className={`${
              submissionResult === "Success!" ? "bg-green-100" : "bg-red-500"
            } p-4 m-3 text-center rounded-md shadow-md text-lg transition-all duration-300 ease-in-out`}
          >
            {submissionResult === "Success!" ? (
              <span className="text-green-800">{submissionResult}</span>
            ) : (
              <span className="text-red-100">{submissionResult}</span>
            )}
          </div>
        )}
        <h3 className="text-center text-lg font-bold m-4">
            Create a Budget
        </h3>
        <div className="mb-4">
          <label
            className="block text-gray-700 text-sm font-bold mb-2"
            htmlFor="categoryId"
          >
            Category
          </label>
          <select
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="categoryId"
            name="categoryId"
            value={formData.categoryId}
            onChange={handleChange}
          >
            {categories.map((category) => (
              //can only create budgets for expenses
              category.type === "Expense" &&
              <option value={category.id} key={category.id}>
                {category.id}) {category.name} {category.icon}
              </option>
            ))}
          </select>
        </div>
        <div className="mb-4">
          <label
            className="block text-gray-700 text-sm font-bold mb-2"
            htmlFor="amount"
          >
            Amount
          </label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            type="number"
            id="amount"
            name="amount"
            value={formData.amount}
            onChange={handleChange}
          />
        </div>
        <div className="mb-4">
          <label
            className="block text-gray-700 text-sm font-bold mb-2"
            htmlFor="month"
          >
            Month
          </label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            type="number"
            id="month"
            name="month"
            value={formData.month}
            onChange={handleChange}
          />
        </div>
        <div className="mb-4">
          <label
            className="block text-gray-700 text-sm font-bold mb-2"
            htmlFor="year"
          >
            Year
          </label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            type="number"
            id="year"
            name="year"
            value={formData.year}
            onChange={handleChange}
          />
        </div>

        <div className="flex items-center justify-between">
          <input
            className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline cursor-pointer"
            type="submit"
            value="Submit"
          />
          <button
            type="button"
            onClick={onClose}
            className="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
