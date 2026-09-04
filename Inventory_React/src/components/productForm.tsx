
import { useState } from "react";

const emptyForm = {
    name: "",
    price: "",
    stock: ""
};

type ProductFormData = {
    name: string;
    price: string;
    stock: string;
};

export default function ProductForm() {
    const [form, setForm] = useState<ProductFormData>(emptyForm);
    const [editingId, setEditingId] = useState<number | null>(null);

    function handleInputChange(event: React.ChangeEvent<HTMLInputElement>) {
        const { name, value } = event.target;

        setForm((currentForm) => ({
            ...currentForm,
            [name]: value,
        }));
    }

    function resetForm() {
        setForm(emptyForm);
        setEditingId(null);
    }

    async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();

        if (!form.name || form.price === "" || form.stock === "") {
            alert("Please complete all fields.");
            return;
        }

        const productData = {
            name: form.name.trim(),
            price: Number(form.price),
            stock: Number(form.stock)
        };

        try {
            const response = await fetch("http://localhost:5293/api/products", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(productData),
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || "Failed to create product");
            }

            alert("Product created successfully.");
            resetForm();

        } catch (error) {
            console.error("Error creating product:", error);
            alert("Could not create the product.");
        }

    }

    return (
        <section className="content-grid">
            <form className="product-form card" onSubmit={handleSubmit}>

                <label>
                    Nom

                    <input
                        name="name"
                        value={form.name}
                        onChange={handleInputChange}
                        placeholder="ex. Wireless Mouse"
                        required
                    />
                </label>

                <label>
                    Prix (€)

                    <input
                        type="number"
                        name="price"
                        value={form.price}
                        onChange={handleInputChange}
                        placeholder="ex. 12"
                        min="0"
                        step="0.01"
                        required
                    />
                </label>

                <label>
                    Quantité

                    <input
                        type="number"
                        name="stock"
                        value={form.stock}
                        onChange={handleInputChange}
                        placeholder="ex. 52"
                        min="0"
                        required
                    />
                </label>

                <div className="form-actions">
                    <button type="submit" className="primary-button">
                        {editingId ? "Sauvegarder" : "Ajouter le produit"}
                    </button>

                    {editingId && (
                        <button
                            type="button"
                            className="secondary-button"
                            onClick={resetForm}
                        >
                            Annuler
                        </button>
                    )}
                </div>
            </form>
        </section>
    );
}