




import { useState, useEffect } from "react";
import { Budget, Category } from "../../types";
import api from "../../services/api";
import Confirmation from "../../components/Confirmation";
import EditBudgetForm from "./EditBudgetForm";
import CreateBudgetForm from "./CreateBudgetForm";

interface budgetListProps {
  //define onFormOpen so that it passes in a transction id
}

export default function BudgetList({}: budgetListProps) {
  const [budgets, setBudgets] = useState<Budget[]>();
  const [isLoading, setIsLoading] = useState(true);
  const [isError, setIsError] = useState(false);
  const [isCreateFormVisible, setIsCreateFormVisible] = useState(false);
  const [isEditFormVisible, setIsEditFormVisible] = useState(false);
  const [editBudgetId, setEditBudgetId] = useState<number>(0);
  const [isConfirmationVisible, setIsConfirmationVisible] = useState(false);
  const [budgetToDelete, setBudgetToDelete] = useState<number>();
  const [selectedMonth, setSelectedMonth] = useState<number | null>(null);
  const [selectedYear, setSelectedYear] = useState<number | null>(null);
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

  const [filters, setFilters] = useState<{
    month: number | null;
    year: number | null;
  }>({
    month: null,
    year: null,
  });

  function applyFilter() {
    setFilters({
      month: selectedMonth,
      year: selectedYear,
    });
  }

  //Reload budget list when filters change
  useEffect(() => {
    const getBudgets = async () => {
      try {
        let url = `/budgets`;
        if (filters.month && filters.year) {
          url += `?month=${filters.month}`;
          url += `&year=${filters.year}`;
        }
        const response = await api.get<Budget[]>(url);
        console.log(response);
        setBudgets(response.data);
      } catch (error) {
        setIsError(true);
        //console.log(error);
      } finally {
        setIsLoading(false);
      }
    };

    getBudgets();
  }, [filters, isLoading]);

  if (isLoading) {
    return <div>Loading budgets...</div>;
  }

  if (isError) {
    return <div>Error fetching budgets.</div>;
  }

  function handleOpenCreateForm() {
    setIsCreateFormVisible(true);
  }

  function handleCloseCreateBudgetForm() {
    setIsCreateFormVisible(false);
  }

  function handleOpenEditForm(id: number) {
    setIsEditFormVisible(true);
    setEditBudgetId(id);
  }

  function handleCloseEditBudgetForm() {
    setIsEditFormVisible(false);
    setEditBudgetId(0);
  }

  function handleDeleteBudget(id: number) {
    setIsConfirmationVisible(true);
    setBudgetToDelete(id);
  }

  async function onConfirmDelete() {
    try {
      const response = await api.delete(`/Budgets/${budgetToDelete}`);
      console.log(response);
      setIsConfirmationVisible(false);
      setIsLoading(true);
    } catch (error) {
      console.error(error);
    }
  }

  function onCancelDelete() {
    setIsConfirmationVisible(false);
  }

  return (
    <div className="container mx-auto px-4">
      <div className="overflow-x-auto">
        <div className="filters flex flex-wrap items-center gap-4 mt-4">
          <input
            type="number"
            className="shadow appearance-none border rounded py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="month"
            //?? is called nullish coalescing operator, if selectedMonth is null or undefined, it will evaluate to ""
            value={selectedMonth ?? ""}
            onChange={(e) => setSelectedMonth(e.target.value === "" ? null : +e.target.value)}
          />
          <input
            type="number"
            className="shadow appearance-none border rounded py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="endDate"
            value={selectedYear ?? ""}
            onChange={(e) => setSelectedYear(e.target.value === "" ? null : +e.target.value)}
          />
          <button
            onClick={applyFilter}
            className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Apply Filters
          </button>
          <button
            onClick={handleOpenCreateForm}
            className="bg-green-400 text-white font-bold py-2 px-4 rounded"
          >
            Create Budget
          </button>
        </div>
        {budgets?.length === 0 ? <p>No budgets found for the applied filters</p> :
        <table className="min-w-full bg-white">
          <thead>
            <tr>
              <th className="py-2 px-4 border-b">ID</th>
              <th className="py-2 px-4 border-b">Category</th>
              <th className="py-2 px-4 border-b">Amount</th>
              <th className="py-2 px-4 border-b">Spent</th>
              <th className="py-2 px-4 border-b">Remaining</th>
              <th className="py-2 px-4 border-b">Status</th>
              <th className="py-2 px-4 border-b"></th>
              <th className="py-2 px-4 border-b"></th>
            </tr>
          </thead>
          <tbody>
            {budgets?.map((budget) => (
              <tr key={budget.id}>
                <td className="py-2 px-4 border-b text-center">{budget.id}</td>
                <td className="py-2 px-4 border-b text-center">
                  {budget.categoryName} {budget.categoryIcon}
                </td>
                <td className="py-2 px-4 border-b text-center">
                  ${budget.amount}
                </td>
                <td className="py-2 px-4 border-b text-center">
                  {budget.spent}
                </td>
                <td className="py-2 px-4 border-b text-center">
                  {budget.remaining}
                </td>
                <td className="py-2 px-4 border-b text-center">
                  {budget.status}
                </td>
                <td className="py-2 px-4 border-b text-center">
                  <button
                    className="bg-blue-500 hover:bg-blue-700 text-white py-2 px-4 rounded disabled:opacity-50"
                    onClick={() => handleOpenEditForm(budget.id)}
                  >
                    Edit
                  </button>
                </td>
                <td className="py-2 px-4 border-b text-center">
                  <button
                    className="bg-red-500 hover:bg-red-700 text-white py-2 px-4 rounded disabled:opacity-50"
                    onClick={() => handleDeleteBudget(budget.id)}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>}
      </div>

      {isConfirmationVisible && (
        <Confirmation
          onConfirm={onConfirmDelete}
          onCancel={onCancelDelete}
          message="Are you sure you want to delete this budget? This cannot be undone."
        />
      )}

      {isCreateFormVisible && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center">
          <div className="bg-white p-8 rounded-lg shadow-2xl">
            <CreateBudgetForm
              onClose={handleCloseCreateBudgetForm}
              categories={categories}
            />
          </div>
        </div>
      )}
      {isEditFormVisible && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center">
          <div className="bg-white p-8 rounded-lg shadow-2xl">
            <EditBudgetForm
              onClose={handleCloseEditBudgetForm}
              budgetId={editBudgetId}
            />
          </div>
        </div>
      )}
    </div>
  );
}
