import { useState, useEffect } from "react";
import { PaginatedTransactions } from "../types";
import api from "../services/api";

export default function TransactionList() {
  const [paginatedTransactions, setTransactions] =
    useState<PaginatedTransactions>();
  const [isLoading, setIsLoading] = useState(true);
  const [isError, setIsError] = useState(false);
  const [pageNumber, setPageNumber] = useState(1);

  useEffect(() => {
    const getTransactions = async () => {
      try {
        const response = await api.get<PaginatedTransactions>(
          `/transactions?pageNumber=${pageNumber}`
        );
        setTransactions(response.data);
      } catch (error) {
        setIsError(true);
        //console.log(error);
      } finally {
        setIsLoading(false);
      }
    };

    getTransactions();
  }, [pageNumber]);

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
    setPageNumber(pageNumber < paginatedTransactions!.totalPages ? pageNumber + 1 : pageNumber);
  }

  return (
    <div className="container mx-auto px-4">
      <div className="overflow-x-auto">
        <table className="min-w-full bg-white">
          <thead>
            <tr>
              <th className="py-2 px-4 border-b">ID</th>
              <th className="py-2 px-4 border-b">Date</th>
              <th className="py-2 px-4 border-b">Amount</th>
              <th className="py-2 px-4 border-b">Type</th>
              <th className="py-2 px-4 border-b">Category</th>
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
    </div>
  );
}
