import { useState, useEffect } from "react";
import { Transaction, Category, CreateTransaction } from "../types";
import api from "../services/api";

interface ConfirmationProps{
    onConfirm: () => void;
    onCancel: () => void;
    message?: string;
}

export default function Confirmation({onConfirm, onCancel, message} : ConfirmationProps) {
  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg shadow-xl max-w-md w-full p-6">
        <h2 className="text-xl font-semibold text-gray-900 mb-2">
          Confirm Action
        </h2>
        <p className="text-gray-600 mb-6">
          {message ? message : "Are you sure you want to proceed with this action? This cannot be undone"}
        </p>
        <div className="flex gap-3 justify-end">
          <button
            onClick={onCancel}
            className="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
          >
            Cancel
          </button>
          <button
            onClick={onConfirm}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            Confirm
          </button>
        </div>
      </div>
    </div>
  );
}
