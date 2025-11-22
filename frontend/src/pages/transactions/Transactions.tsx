import React, { useState, useEffect } from "react";
import TransactionList from "./TransactionList";
import TransactionForm from "./TransactionForm";
import { Category } from "../../types";
import api from "../../services/api";

function Transactions() {
  const [isFormVisible, setIsFormVisible] = useState(false);
  const [formType, setFormType] = useState("create");
  const [editingTransactionId, setEditingTransactionId] = useState<
    number | null
  >(null);
  const [categories, setCategories] = useState<Category[]>([
    {
      id: 1,
      name: "Food & Dining",
      type: "Expense",
      icon: "🍔",
      isCustom: false,
    },
  ]);

  useEffect(() => {
    const getCategories = async () => {
      try {
        const response = await api.get<Category[]>(`/categories`);
        setCategories(response.data);
      } catch (error) {
        //console.log(error);
      }
    };
    getCategories();
  }, []);

  function handleOpenForm(type: string, id?: number) {
    setIsFormVisible(true);
    setFormType(type);
    setEditingTransactionId(id || null);
  }

  function handleCloseForm() {
    setIsFormVisible(false);
    setEditingTransactionId(null);
    setFormType("create");
  }

  return (
    <>
      <title>Transactions</title>
      <div className="bg-white rounded-lg shadow p-6">
        <div className="flex items-center justify-between mt-4">
          <p className="text-2xl font-bold mb-4">Transactions</p>
          <button
            onClick={() => handleOpenForm("create")}
            className="bg-green-400 text-white font-bold py-2 px-4 rounded"
          >
            Create Transaction
          </button>
        </div>
        {/* onFormOpen is a function prop that when you call it in TransactionList
              you need to pass in the an id, and when it is called it will call handleOpenForm
          */}
        <TransactionList
          onFormOpen={(id) => handleOpenForm("edit", id)}
          categories={categories}
        />
      </div>

      {isFormVisible && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center">
          <div className="bg-white p-8 rounded-lg shadow-2xl">
            <TransactionForm
              onClose={handleCloseForm}
              formType={formType}
              transactionId={editingTransactionId}
              categories={categories}
            />
          </div>
        </div>
      )}
    </>
  );
};

export default Transactions;
