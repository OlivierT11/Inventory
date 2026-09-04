import ProductForm from '../components/productForm';

export default function Product_Add() {
    return (
        <>
            <div className="card-header">
                <h1>Ajouter un produit à l'inventaire</h1>
            </div>
            <section className="product-list-container">
                <ProductForm />
            </section>
        </>
    );
}