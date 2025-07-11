import React, { useState } from "react";
import "./App.css";

function App() {
    const [ingredients, setIngredients] = useState({
        Cucumber: 2,
        Olives: 2,
        Lettuce: 3,
        Meat: 6,
        Tomato: 6,
        Cheese: 8,
        Dough: 10,
    });

    const [result, setResult] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const handleOptimize = async () => {
        setLoading(true);
        setError("");

        try {
            const res = await fetch("https://localhost:7287/api/optimize", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ ingredients }),
            });

            if (!res.ok) {
                throw new Error("Server error: " + res.status);
            }

            const data = await res.json();
            setResult(data);
        } catch (err) {
            setError("Failed to fetch optimization results.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="app">
            <h1>🍽️ Optimal Meal Planner</h1>
            <p>Get the best combination of meals using your current ingredients.</p>

            <button onClick={handleOptimize} disabled={loading}>
                {loading ? "Optimizing..." : "Optimize Meals"}
            </button>

            {error && <p className="error">{error}</p>}

            {result && (
                <div className="results">
                    <h2>Total People Fed: {result.totalPeopleFed}</h2>
                    <div className="recipe-list">
                        {Object.entries(result.recipeCounts).map(([recipe, count]) => (
                            <div key={recipe} className="recipe-item">
                                <strong>{recipe}</strong>: {count}
                            </div>
                        ))}
                    </div>
                </div>
            )}
        </div>
    );
}

export default App;
