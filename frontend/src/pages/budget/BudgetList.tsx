import { useState, useEffect } from "react";
import { Budget, Category } from "../../types";
import api from "../../services/api";
import Confirmation from "../../components/Confirmation";
import EditBudgetForm from "./EditBudgetForm";
import CreateBudgetForm from "./CreateBudgetForm";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";

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
  const [selectedMonth, setSelectedMonth] = useState<number | null>(
    new Date().getMonth() + 1
  );
  const [selectedYear, setSelectedYear] = useState<number | null>(
    new Date().getFullYear()
  );
  const [categories, setCategories] = useState<Category[]>([
    {
      id: 1,
      name: "Food & Dining",
      type: "Expense",
      icon: "🍔",
      isCustom: false,
    },
  ]);
  const [filters, setFilters] = useState<{
    month: number | null;
    year: number | null;
  }>({
    month: new Date().getMonth() + 1,
    year: new Date().getFullYear(),
  });


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

  function applyFilter() {
    setFilters({
      month: selectedMonth,
      year: selectedYear,
    });
  }

  //Reload budget list when filters change..
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
      <div>
        <div className="filters flex flex-wrap items-center gap-4 mt-4 mb-6">
          <input
            type="number"
            className="shadow appearance-none border rounded py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            placeholder="Month"
            id="month"
            value={selectedMonth ?? ""}
            onChange={(e) =>
              setSelectedMonth(e.target.value === "" ? null : +e.target.value)
            }
          />
          <input
            type="number"
            className="shadow appearance-none border rounded py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            placeholder="Year"
            id="year"
            value={selectedYear ?? ""}
            onChange={(e) =>
              setSelectedYear(e.target.value === "" ? null : +e.target.value)
            }
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

        <div className="text-xl font-bold text-gray-800 mb-4">
          Showing Budgets for {filters.month}/{filters.year}
        </div>

        {budgets?.length === 0 ? (
          <p>No budgets found for the applied filters</p>
        ) : (
          budgets?.map((budget) => (
            <div
              key={budget.id}
              className="bg-white shadow-md rounded-lg p-6 mb-4 border"
            >
              <div className="flex justify-between items-center mb-4">
                <h3 className="text-lg font-semibold flex items-center">
                  {budget.categoryName} {budget.categoryIcon}
                </h3>
                <div className="flex gap-2">
                  <button
                    className="bg-blue-500 hover:bg-blue-700 text-white py-1 px-3 rounded text-sm"
                    onClick={() => handleOpenEditForm(budget.id)}
                  >
                    Edit
                  </button>
                  <button
                    className="bg-red-500 hover:bg-red-700 text-white py-1 px-3 rounded text-sm"
                    onClick={() => handleDeleteBudget(budget.id)}
                  >
                    Delete
                  </button>
                </div>
              </div>
              <div className="mb-3">
                <ResponsiveContainer width="100%" height={30}>
                  <BarChart
                    layout="vertical"
                    data={[
                      {
                        spent: Math.min(budget.percentageUsed, 100),
                        remaining: Math.max(0, 100 - budget.percentageUsed),
                      },
                    ]}
                    margin={{ top: 0, right: 0, left: 0, bottom: 0 }}
                  >
                    <XAxis type="number" hide domain={[0, 100]} />
                    <YAxis type="category" hide />
                    <Bar
                      dataKey="spent"
                      stackId="a"
                      fill={
                        budget.status === "Under"
                          ? "#10b981"
                          : budget.status === "Near"
                          ? "#f59e0b"
                          : "#ef4444"
                      }
                      radius={[5, 0, 0, 5]}
                    />
                    <Bar
                      dataKey="remaining"
                      stackId="a"
                      fill="#e5e7eb"
                      radius={[0, 5, 5, 0]}
                    />
                  </BarChart>
                </ResponsiveContainer>
              </div>
              <div className="flex justify-between text-sm text-gray-600 mb-2">
                <span>${budget.spent} spent</span>
                <span>${budget.remaining} remaining</span>
              </div>
              <div className="flex justify-between items-center">
                <span>Total: ${budget.amount}</span>
                <span
                  className={`px-2 py-1 rounded text-xs font-semibold ${
                    budget.status === "Under"
                      ? "bg-green-100 text-green-800"
                      : budget.status === "Near"
                      ? "bg-yellow-100 text-yellow-800"
                      : "bg-red-100 text-red-800"
                  }`}
                >
                  {budget.status}
                </span>
              </div>
            </div>
          ))
        )}
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
