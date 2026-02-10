from flask import Flask, request, jsonify
import joblib
import numpy as np

# --------------------------------
# Create Flask app
# --------------------------------
app = Flask(__name__)

# --------------------------------
# Load trained ML model
# --------------------------------
model = joblib.load("performance_model.pkl")

# --------------------------------
# Home route
# --------------------------------
@app.route("/", methods=["GET"])
def home():
    return "Cloud ML Performance API is running"

# --------------------------------
# Prediction route
# --------------------------------
@app.route("/predict", methods=["POST"])
def predict():
    try:
        data = request.get_json()

        # Extract input values
        score = float(data["score"])
        response_time = float(data["response_time"])
        attempts = int(data["attempts"])

        # ✅ Rule-based override
        if score >= 90 and response_time <= 1.5 and attempts == 0:
            return jsonify({"performance": "Good"})

        # ML prediction
        X = np.array([[score, response_time, attempts]])
        prediction = model.predict(X)[0]

        # Convert prediction to label
        if prediction == 2:
            performance = "Good"
        elif prediction == 1:
            performance = "Average"
        else:
            performance = "Poor"

        return jsonify({"performance": performance})

    except Exception as e:
        return jsonify({"error": str(e)}), 500

# --------------------------------
# Run app (for cloud + local)
# --------------------------------
if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)
