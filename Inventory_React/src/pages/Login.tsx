import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

type User= {
    email: string;
    password: string;
};

type UserFormData = {
    email: string;
    password: string;
};

const emptyForm: UserFormData = {
    email: "",
    password: "",
};


export default function LoginUser() {
    const [user, setUser] = useState<User>();
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const [form, setForm] = useState<UserFormData>(emptyForm);
    const { login } = useAuth();

    function handleInputChange(event: React.ChangeEvent<HTMLInputElement>) {
        const { name, value } = event.target;

        setForm((currentForm) => ({
            ...currentForm,
            [name]: value,
        }));
    }

    async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();
        console.log("User to login:", form);

        // Validate
        const email = form.email.trim();
        const password = form.password.trim();

        if (!email || !password) {
            alert("Please complete all fields.");
            return;
        }

        const user = {
            email: email,
            password: password,
        };

        const apiUrl = import.meta.env.VITE_API_URL;

        try {
            setLoading(true);

            const response = await fetch(`${apiUrl}/api/auth/login`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(user),
            });

            if (!response.ok) {
                const message = await response.text();
                throw new Error(message || "Unable to login.");
            }


            const data: { access_token: string } = await response.json();

            if (!data.access_token) {
                throw new Error("The server did not return a JWT.");
            }

            login(data.access_token);

            // Optional: redirect after login
            navigate("/Product_Add", { replace: true });

            // alert("Login successfully.");
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }

    return (
        <section className="login">
            <div className="card-header">
                <h1>Login</h1>
            </div>

            {error && <p>{error}</p>}

            {/* ensure `loading` is read so TypeScript doesn't flag it as unused */}
            {loading && <p>Loading…</p>}

            <form className="product-form card" onSubmit={handleSubmit}>
                <div className="action-buttons">
                    <input
                        type="text"
                        name="email"
                        value={form.email}
                        onChange={handleInputChange}
                    />
                    <input
                        type="password"
                        name="password"
                        value={form.password}
                        onChange={handleInputChange}
                    />
                    <button className="login-button">
                        Login
                    </button>

                </div>
            </form>
        </section>
    );
}