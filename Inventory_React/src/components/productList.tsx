import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

type Product = {
    id: number;
    name: string;
    price: number;
    stock: number;
};

export default function ProductList() {
    const [products, setProducts] = useState<Product[]>([]);
    const [error, setError] = useState("");
    const [editingId, setEditingId] = useState<number | null>(null);
    const [loading, setLoading] = useState(false);
    const [currentPage, setCurrentPage] = useState(1);
    const [, setPageSize] = useState(10);
    const [, setTotalItems] = useState(0);
    const [totalPages, setTotalPages] = useState(0);
    const navigate = useNavigate();


    function cancelEditing() {
        setEditingId(null);
    }

    function handleProductChange(id: number, event: React.ChangeEvent<HTMLInputElement>) {
        const { name, value } = event.target;

        setProducts((currentProducts) =>
            currentProducts.map((product) =>
                product.id === id
                    ? {
                        ...product,
                        [name]: value,
                    }
                    : product
            )
        );
    }

    async function handleEdit(product: Product) {
        // event.preventDefault();
        console.log("Product to update:", product);

        // Validate
        const productId = Number(product.id);
        const productName = product.name.trim();
        const priceNum = Number(product.price);
        const stockNum = Number(product.stock);

        if (!product.name || Number.isNaN(priceNum) || Number.isNaN(stockNum)) {
            alert("Please complete all fields.");
            return;
        }

        const updatedProduct = {
            name: productName,
            price: priceNum,
            stock: stockNum,
        };

        const apiUrl = import.meta.env.VITE_API_URL;

        try {
            setLoading(true);

            const response = await fetch(`${apiUrl}/api/products/${productId}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(updatedProduct),
            });

            if (!response.ok) {
                const message = await response.text();
                throw new Error(message || "Unable to update product");
            }

            // If the API returns the updated product
            const productFromApi = await response.json();

            setProducts((currentProducts) =>
                currentProducts.map((product) =>
                    product.id === editingId ? productFromApi : product
                )
            );

            cancelEditing();
            alert("Product updated successfully.");
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }

    async function handleDelete(id: number) {
        const confirmed = window.confirm(
            "Are you sure you want to delete this product?"
        );

        const apiUrl = import.meta.env.VITE_API_URL;

        if (!confirmed) return;

        try {
            setLoading(true);

            const response = await fetch(`${apiUrl}/api/products/${id}`, {
                method: "DELETE",
            });

            if (!response.ok) {
                const message = await response.text();
                throw new Error(message || "Unable to delete product");
            }

            setProducts((currentProducts) =>
                currentProducts.filter((product) => product.id !== id)
            );

            if (editingId === id) {
                cancelEditing();
            }
            alert("Product deleted successfully.");
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }


    useEffect(() => {

        const params = new URLSearchParams({
            page: String(currentPage)
        });

        const apiUrl = import.meta.env.VITE_API_URL;

        fetch(`${apiUrl}/api/products/paged?${params}`)
            .then((response) => {
                if (!response.ok) {
                    throw new Error(`HTTP error: ${response.status}`);
                }

                return response.json();
            })
            .then((data) => {
                setProducts(data.products);
                setCurrentPage(data.currentPage);
                setPageSize(data.pageSize);
                setTotalItems(data.totalItems);
                setTotalPages(data.totalPages);
            })
            .catch((error) => {
                setError(error.message);
            });
    }, [currentPage]);

    return (
        <section className="products">
            <div className="card-header">
                <h1>Liste des produits de l'inventaire</h1>
            </div>

            {error && <p>{error}</p>}

            {/* ensure `loading` is read so TypeScript doesn't flag it as unused */}
            {loading && <p>Loading…</p>}

            <section className="content-grid">
                <div className="product-list">
                    <ul>
                        {products.map((product) => (
                            <li key={product.id}>
                                <form className="product-form card" onSubmit={(event) => event.preventDefault()}>
                                    <div className="action-buttons">
                                        <button
                                            onClick={() => navigate(`/Product_Details/${product.id}`)}>
                                            Détails
                                        </button>
                                        <input
                                            type="text"
                                            name="name"
                                            value={product.name}
                                            onChange={(event) =>
                                                handleProductChange(product.id, event)
                                            }
                                        />
                                        <input
                                            type="number"
                                            name="price"
                                            value={product.price}
                                            onChange={(event) =>
                                                handleProductChange(product.id, event)
                                            }
                                        />
                                        <input
                                            type="number"
                                            name="stock"
                                            value={product.stock}
                                            onChange={(event) =>
                                                handleProductChange(product.id, event)
                                            }
                                        />
                                        <button
                                            className="edit-button"
                                            onClick={() => handleEdit(product)}
                                        >
                                            Modifier
                                        </button>

                                        <button
                                            className="delete-button"
                                            onClick={() => handleDelete(product.id)}
                                        >
                                            Supprimer
                                        </button>
                                    </div>
                                </form>
                            </li>
                        ))}
                    </ul>
                </div>
            </section>
            <section className="pagination">
                {totalPages > 0 && (
                    <div>
                        <button
                            disabled={currentPage === 1}
                            onClick={() => setCurrentPage(currentPage - 1)}
                        >
                            Précédent
                        </button>

                        {Array.from({ length: totalPages }, (_, index) => {
                            const pageNumber = index + 1;

                            return (
                                <button
                                    key={pageNumber}
                                    onClick={() => setCurrentPage(pageNumber)}
                                    style={{
                                        fontWeight: pageNumber === currentPage ? "bold" : "normal"
                                    }}
                                >
                                    {pageNumber}
                                </button>
                            );
                        })}

                        <button
                            disabled={currentPage === totalPages}
                            onClick={() => setCurrentPage(currentPage + 1)}
                        >
                            Suivant
                        </button>

                        <p>
                            Page {currentPage} de {totalPages}
                        </p>
                    </div>
                )}
            </section>
        </section>
    );
}