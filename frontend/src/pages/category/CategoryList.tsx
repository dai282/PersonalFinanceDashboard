import { useState, useEffect } from "react";
import { Category } from "../../types";
import api from "../../services/api";
import Confirmation from "../../components/Confirmation";
import CreateEditCategoryForm from "./CreateEditCategoryForm";

export default function CategoryList() {
  const [categories, setCategories] = useState<Category[]>();
  const [isLoading, setIsLoading] = useState(true);
  const [isError, setIsError] = useState(false);
  const [formType, setFormType] = useState("create");
  const [isCreateEditFormVisible, setIsCreateEditFormVisible] = useState(false);
  const [editCategoryId, setEditCategoryId] = useState<number | null>(null);
  const [isConfirmationVisible, setIsConfirmationVisible] = useState(false);
  const [categoryToDelete, setcategoryToDelete] = useState<number>();
  const [pressedSubmit, setpressedSubmit] = useState(false);


  useEffect(() => {
    const GetCategoriess = async () => {
      try {
        const response = await api.get<Category[]>("/Categories");
        console.log(response);
        setCategories(response.data);
      } catch (error) {
        setIsError(true);
        //console.log(error);
      } finally {
        setIsLoading(false);
      }
    };

    GetCategoriess();
  }, [isLoading, formType, isConfirmationVisible,pressedSubmit ]);

  if (isLoading) {
    return <div>Loading categories...</div>;
  }

  if (isError) {
    return <div>Error fetching categories.</div>;
  }

  function handleOpenCreateEditCategoryForm(id?: number) {
    setIsCreateEditFormVisible(true);
    id && setEditCategoryId(id);
    id ? setFormType('edit') : setFormType('create');
  }

  function handleCloseCreateEditCategoryForm() {
    setIsCreateEditFormVisible(false);
    setFormType('create')
  }

  function handleDeleteCategory(id: number) {
    setIsConfirmationVisible(true);
    setcategoryToDelete(id);
  }

  async function onConfirmDelete() {
    try {
      const response = await api.delete(`/Categories/${categoryToDelete}`);
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
      <button
        onClick={() => handleOpenCreateEditCategoryForm()}
        className="bg-green-400 text-white font-bold py-2 px-4 rounded mb-3"
      >
        Create Category
      </button>
      <div className="grid sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
        {categories?.map((category) => (
          <div
            key={category.id}
            className="bg-white shadow-md rounded-lg p-6 mb-4 border"
          >
            <h3 className="text-lg text-center font-semibold flex items-center">
              {category.icon}
            </h3>
            <div className="text-lg font-semibold flex items-center">
              {category.name}
            </div>
            <div
              className={`px-2 py-1 rounded text-xs font-semibold mt-2 mb-2 ${
                category.type === "Income"
                  ? "bg-green-100 text-green-800"
                  : "bg-red-100 text-red-800"
              }`}
            >
              {category.type}
            </div>
            {/*Can only Edit and Delete custom categories*/}
            {category.isCustom === true && (
              <div className="flex justify-between items-center mb-4">
                <div className="flex gap-2">
                  <button
                    className="bg-blue-500 hover:bg-blue-700 text-white py-1 px-3 rounded text-sm"
                    onClick={() =>
                      handleOpenCreateEditCategoryForm(category.id)
                    }
                  >
                    Edit
                  </button>
                  <button
                    className="bg-red-500 hover:bg-red-700 text-white py-1 px-3 rounded text-sm"
                    onClick={() => handleDeleteCategory(category.id)}
                  >
                    Delete
                  </button>
                </div>
              </div>
            )}
          </div>
        ))}
      </div>

      {isConfirmationVisible && (
        <Confirmation
          onConfirm={onConfirmDelete}
          onCancel={onCancelDelete}
          message="Are you sure you want to delete this category? This cannot be undone."
        />
      )}

      {isCreateEditFormVisible && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center">
          <div className="bg-white p-8 rounded-lg shadow-2xl">
            <CreateEditCategoryForm
              onClose={handleCloseCreateEditCategoryForm}
              categoryId={editCategoryId}
              formType={formType}
              setpressedSubmit={(submit) =>setpressedSubmit(submit)}
              pressedSubmit ={pressedSubmit}
            />
          </div>
        </div>
      )}
    </div>
  );
}
