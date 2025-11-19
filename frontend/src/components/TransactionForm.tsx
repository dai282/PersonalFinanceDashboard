import { useState, useEffect } from "react";
import { Transaction, Category, CreateTransaction } from "../types";
import api from "../services/api";

interface TransactionFormProps {
  onClose: () => void;
  formType: string;
  transactionId?: number | null;
  categories: Category[];
}

export default function TransactionForm({ onClose , formType, transactionId, categories}: TransactionFormProps) {
  const [formData, setFormData] = useState<CreateTransaction>({
    categoryId: 0,
    amount: 0,
    description: "",
    transactionDate: new Date(),
  });

  const [pressedSubmit, setpressedSubmit] = useState(false);
  const [submissionResult, setSubmissionResult] = useState<string>();

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value, type } = e.target;
    const parsedValue =
      type === "number" || name === "categoryId"
        ? Number(value)
        : //if the date is changed, transform it back to Date format instead of ISO string
        type === "date"
        ? new Date(value)
        : value;

    setFormData((prevState) => ({
      ...prevState!,
      [name]: parsedValue,
    }));
  };

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setpressedSubmit(true);
    setTimeout(() => {
      setpressedSubmit(false);
    }, 3000);
    if (formData.amount <= 0) {
      setSubmissionResult("Amount must be greater than 0!");
      return;
    }
    try {
      setSubmissionResult("Success!");
      const response = await formType === 'create'
        ? api.post("/transactions", formData)
        : api.put(`/transactions/${transactionId}`, formData);
      console.log(response);
    } catch (error) {
      console.error(error);
    }
  }


  //when a formType and transactionId is changed (i.e when the edit button is clicked), 
  //get the transaction data and map it to the form fields
  useEffect(() => {
    const getTransaction = async () => {
      if (formType === 'edit' && transactionId) {
        try {
          const response = await api.get<Transaction>(`/transactions/${transactionId}`);
          const transaction = response.data;
          setFormData({
            categoryId: transaction.categoryId,
            amount: transaction.amount,
            description: transaction.description,
            transactionDate: new Date(transaction.transactionDate),
          });
        } catch (error) {
          console.error(error);
        }
      }
    };
    getTransaction();
  }, [formType, transactionId]);

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
          {formType === 'create'? "Create a Transaction" : "Edit a Transaction"}
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
            htmlFor="description"
          >
            Description
          </label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            type="text"
            id="description"
            name="description"
            value={formData.description}
            onChange={handleChange}
          />
        </div>
        <div className="mb-4">
          <label
            className="block text-gray-700 text-sm font-bold mb-2"
            htmlFor="transactionDate"
          >
            Transaction Date
          </label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            type="date"
            id="transactionDate"
            name="transactionDate"
            value={formData.transactionDate.toISOString().split("T")[0]}
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
