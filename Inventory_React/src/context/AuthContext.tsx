import {
    createContext,
    useContext,
    useEffect,
    useState,
    type PropsWithChildren,
} from "react";

type AuthContextValue = {
    token: string | null;
    isAuthenticated: boolean;
    loading: boolean;
    login: (newToken: string) => void;
    logout: () => void;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: PropsWithChildren) {
    const [token, setToken] = useState<string | null>(() =>
        localStorage.getItem("access_token")
    );

    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        async function checkAuthentication(): Promise<void> {
            if (!token) {
                setIsAuthenticated(false);
                setLoading(false);
                return;
            }

            const apiUrl = import.meta.env.VITE_API_URL;

            try {
                const response = await fetch(`${apiUrl}/api/profile`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });

                if (!response.ok) {
                    throw new Error("Token invalide ou expiré");
                }

                setIsAuthenticated(true);
            } catch {
                localStorage.removeItem("access_token");
                setToken(null);
                setIsAuthenticated(false);
            } finally {
                setLoading(false);
            }
        }

        checkAuthentication();
    }, [token]);

    function login(newToken: string): void {
        localStorage.setItem("access_token", newToken);
        setToken(newToken);
        setIsAuthenticated(true);
        setLoading(false);
    }

    function logout(): void {
        localStorage.removeItem("access_token");
        setToken(null);
        setIsAuthenticated(false);
    }

    return (
        <AuthContext.Provider
            value={{
                token,
                isAuthenticated,
                loading,
                login,
                logout,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth(): AuthContextValue {
    const context = useContext(AuthContext);

    if (!context) {
        throw new Error("useAuth doit être utilisé dans un AuthProvider");
    }

    return context;
}