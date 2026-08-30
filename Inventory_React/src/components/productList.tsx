import { useEffect, useState } from "react";

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
        const priceNum = Number(product.price);
        const stockNum = Number(product.stock);

        if (!product.name || Number.isNaN(priceNum) || Number.isNaN(stockNum)) {
            alert("Please complete all fields.");
            return;
        }

        const updatedProduct = {
            name: product.name.trim(),
            price: Number(product.price),
            stock: Number(product.stock),
        };

        try {
            setLoading(true);

            const response = await fetch(`http://localhost:5293/api/products/${product.id}`, {
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

        if (!confirmed) return;

        try {
            setLoading(true);

            const response = await fetch(`http://localhost:5293/api/products/${id}`, {
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
        fetch("http://localhost:5293/api/products")
            .then((response) => {
                if (!response.ok) {
                    throw new Error(`HTTP error: ${response.status}`);
                }

                return response.json();
            })
            .then((data) => {
                setProducts(data);
            })
            .catch((error) => {
                setError(error.message);
            });
    }, []);

    return (
        <section className="products">
            <h1>Products</h1>

            {error && <p>{error}</p>}

            {/* ensure `loading` is read so TypeScript doesn't flag it as unused */}
            {loading && <p>Loading…</p>}

            <ul>
                {products.map((product) => (
                    <li key={product.id}>
                        <form className="product-form card" onSubmit={(event) => event.preventDefault()}>
                            <div className="action-buttons">
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
                                    Edit
                                </button>

                                <button
                                    className="delete-button"
                                    onClick={() => handleDelete(product.id)}
                                >
                                    Delete
                                </button>
                            </div>
                        </form>
                    </li>
                ))}
            </ul>
        </section>
    );
}