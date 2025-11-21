import { useState, useEffect } from "react";
import {
  Transaction,
  Category,
  CreateTransaction,
  CreateEditCategory,
} from "../../types";
import api from "../../services/api";

interface CreateEditCategoryFormProps {
  onClose: () => void;
  setpressedSubmit: (submit:boolean) => void;
  pressedSubmit: boolean;
  formType: string;
  categoryId?: number | null;
}

export default function CreateEditCategoryForm({
  onClose,
  setpressedSubmit,
  pressedSubmit,
  formType,
  categoryId,
}: CreateEditCategoryFormProps) {
  const [formData, setFormData] = useState<CreateEditCategory>({
    name: "",
    type: "Income",
    icon: "",
  });
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
    try {
      setSubmissionResult("Success!");
      const response =
        (await formType) === "create"
          ? api.post("/Categories", formData)
          : api.put(`/Categories/${categoryId}`, formData);
      console.log(response);
    } catch (error) {
      console.error(error);
    }
  }

  //when a formType and categoryId is changed (i.e when the edit button is clicked),
  //get the transaction data and map it to the form fields
  useEffect(() => {
    const getCategory = async () => {
      if (formType === "edit" && categoryId) {
        try {
          const response = await api.get<Category>(`/Categories/${categoryId}`);
          const category = response.data;
          setFormData({
            name: category.name,
            type: category.type,
            icon: category.icon,
          });
        } catch (error) {
          console.error(error);
        }
      }
    };
    getCategory();
  }, [formType, categoryId]);

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
          {formType === "create" ? "Create Category" : "Edit Category"}
        </h3>
        <div className="mb-4">
          <label
            className="block text-gray-700 text-sm font-bold mb-2"
            htmlFor="name"
          >
            Category Name
          </label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            type="text"
            id="name"
            name="name"
            value={formData.name}
            onChange={handleChange}
          />
        </div>
        <div className="mb-4">
          <label
            className="block text-gray-700 text-sm font-bold mb-2"
            htmlFor="type"
          >
            Type
          </label>
          <select
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="type"
            name="type"
            value={formData.type}
            onChange={handleChange}
          >
            <option value="Income">Income</option>
            <option value="Expense">Expense</option>
          </select>
        </div>
        <div className="mb-4">
          <label
            className="block text-gray-700 text-sm font-bold mb-2"
            htmlFor="icon"
          >
            Icon
          </label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            type="text"
            id="icon"
            name="icon"
            value={formData.icon}
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
