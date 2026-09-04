import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";

type Product = {
    id: number;
    name: string;
    price: number;
    stock: number;
};


export default function Product_Details() {

    const [product, setProduct] = useState<Product>();
    const [, setError] = useState("");

    const { id } = useParams();

    useEffect(() => {
        fetch(`http://localhost:5293/api/products/${id}`)
            .then((response) => {
                if (!response.ok) {
                    throw new Error(`HTTP error: ${response.status}`);
                }

                return response.json();
            })
            .then((data) => {
                setProduct(data);
            })
            .catch((error) => {
                setError(error.message);
            });
    }, []);

    return (
        <>
            <div className="card-header">
                <h1>Détails du produit</h1>
            </div>
            <section className="product-details-container">
                <form className="product-form card">
                    <p>Id: {product?.id}</p><br/>
                    <p>Nom: {product?.name}</p><br/>
                    <p>Prix: {product?.price}</p><br />
                    <p>Stock: {product?.stock}</p><br />
                </form>
            </section>
        </>
    );
}