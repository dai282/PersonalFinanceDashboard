import { useState, useEffect } from "react";
import { Category, PaginatedTransactions } from "../types";
import api from "../services/api";
import Confirmation from "./Confirmation";

interface TransactionListProps {
  //define onFormOpen so that it passes in a transction id
  onFormOpen: (id?: number) => void;
  categories: Category[];
}

export default function TransactionList({onFormOpen, categories,}: TransactionListProps) {
  const [paginatedTransactions, setTransactions] =
    useState<PaginatedTransactions>();
  const [isLoading, setIsLoading] = useState(true);
  const [isError, setIsError] = useState(false);
  const [pageNumber, setPageNumber] = useState(1);
  const [isConfirmationVisible, setIsConfirmationVisible] = useState(false);
  const [transactionToDelete, setTransactionToDelete] = useState<number>();
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
  const [selectedStartDate, setSelectedStartDate] = useState<string>("");
  const [selectedEndDate, setSelectedEndDate] = useState<string>("");
  const [filters, setFilters] = useState<{
    category: Category | null;
    startDate: Date | null;
    endDate: Date | null;
  }>({
    category: selectedCategory,
    startDate: null,
    endDate: null,
  });

  function applyFilter() {
    setFilters({
      ...filters,
      category: selectedCategory,
      startDate: selectedStartDate ? new Date(selectedStartDate) : null,
      endDate: selectedEndDate ? new Date(selectedEndDate) : null,
    });
    setPageNumber(1); // Reset to first page when applying filters
  }

  function handleFilterChange(event: React.ChangeEvent<HTMLSelectElement>) {
    const value = event.target.value;
    if (value === "all") {
      setSelectedCategory(null);
    } else {
      const category = categories.find((cat) => cat.id === parseInt(value));
      if (category) {
        setSelectedCategory(category);
      }
    }
  }

  //Reload transaction list when pagination buttons are clicked or filters change
  useEffect(() => {
    const getTransactions = async () => {
      try {
        let url = `/transactions?pageNumber=${pageNumber}`;
        if (filters.category) {
          url += `&categoryId=${filters.category.id}`;
        }
        if (filters.startDate) {
          url += `&startDate=${filters.startDate.toISOString().slice(0, 10)}`;
        }
        if (filters.endDate) {
          url += `&endDate=${filters.endDate.toISOString().slice(0, 10)}`;
        }
        const response = await api.get<PaginatedTransactions>(url);
        setTransactions(response.data);
      } catch (error) {
        setIsError(true);
        //console.log(error);
      } finally {
        setIsLoading(false);
      }
    };

    getTransactions();
  }, [pageNumber, filters, isLoading]);

  if (isLoading) {
    return <div>Loading transactions...</div>;
  }

  if (isError) {
    return <div>Error fetching transactions.</div>;
  }

  function PreviousPage() {
    setPageNumber(pageNumber > 1 ? pageNumber - 1 : pageNumber);
  }

  function NextPage() {
    setPageNumber(
      pageNumber < paginatedTransactions!.totalPages
        ? pageNumber + 1
        : pageNumber
    );
  }

  function handleDeleteTransaction(id: number) {
    setIsConfirmationVisible(true);
    setTransactionToDelete(id);
  }

  async function onConfirmDelete() {
    try {
      const response = await api.delete(`/transactions/${transactionToDelete}`);
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
          <select
            className="shadow appearance-none border rounded py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="categoryId"
            name="categoryId"
            value={selectedCategory ? selectedCategory.id.toString() : "all"}
            onChange={handleFilterChange}
          >
            <option value="all">All Categories</option>
            {categories.map((category) => (
              <option value={category.id.toString()} key={category.id}>
                {category.id}) {category.name} {category.icon}
              </option>
            ))}
          </select>
          <input
            type="date"
            className="shadow appearance-none border rounded py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="startDate"
            value={selectedStartDate}
            onChange={(e) => setSelectedStartDate(e.target.value)}
          />
          <input
            type="date"
            className="shadow appearance-none border rounded py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="endDate"
            value={selectedEndDate}
            onChange={(e) => setSelectedEndDate(e.target.value)}
          />
          <button
            onClick={applyFilter}
            className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Apply Filters
          </button>
        </div>

        <table className="min-w-full bg-white">
          <thead>
            <tr>
              <th className="py-2 px-4 border-b">ID</th>
              <th className="py-2 px-4 border-b">Date</th>
              <th className="py-2 px-4 border-b">Amount</th>
              <th className="py-2 px-4 border-b">Type</th>
              <th className="py-2 px-4 border-b">Category</th>
              <th className="py-2 px-4 border-b"></th>
              <th className="py-2 px-4 border-b"></th>
            </tr>
          </thead>
          <tbody>
            {paginatedTransactions!.transactions.map((transaction) => (
              <tr key={transaction.id}>
                <td className="py-2 px-4 border-b text-center">
                  {transaction.id}
                </td>
                <td className="py-2 px-4 border-b text-center">
                  {new Date(transaction.transactionDate).toLocaleDateString()}
                </td>
                <td className="py-2 px-4 border-b text-center">
                  ${transaction.amount}
                </td>
                <td className="py-2 px-4 border-b text-center">
                  {transaction.type}
                </td>
                <td className="py-2 px-4 border-b text-center">
                  {transaction.categoryName}
                </td>
                <td className="py-2 px-4 border-b text-center">
                  <button
                    className="bg-blue-500 hover:bg-blue-700 text-white py-2 px-4 rounded disabled:opacity-50"
                    onClick={() => onFormOpen(transaction.id)}
                  >
                    Edit
                  </button>
                </td>
                <td className="py-2 px-4 border-b text-center">
                  <button
                    className="bg-red-500 hover:bg-red-700 text-white py-2 px-4 rounded disabled:opacity-50"
                    onClick={() => handleDeleteTransaction(transaction.id)}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <div className="flex items-center justify-between mt-4">
        <button
          onClick={PreviousPage}
          disabled={pageNumber === 1}
          className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Previous
        </button>
        <span>
          Page {pageNumber} of {paginatedTransactions?.totalPages}
        </span>
        <button
          onClick={NextPage}
          disabled={pageNumber === paginatedTransactions?.totalPages}
          className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Next
        </button>
      </div>

      {isConfirmationVisible && (
        <Confirmation
          onConfirm={onConfirmDelete}
          onCancel={onCancelDelete}
          message="Are you sure you want to delete this transaction? This cannot be undone."
        />
      )}
    </div>
  );
}
