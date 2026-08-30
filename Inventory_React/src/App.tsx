import './App.css'
import ProductList from './components/productList';
import ProductForm from './components/productForm';

function App() {

    return (
        <main className="app-container">
            <ProductList />
            <ProductForm />
        </main>
    );
}

export default App
