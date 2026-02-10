from flask import Flask, request, jsonify
import joblib

# Create Flask app
app = Flask(__name__)

# Load trained ML model
# Make sure performance_model.pkl is in the same folder
model = joblib.load("performance_model.pkl")


# -------------------------------
# Home route (test in browser)
# -------------------------------
@app.route("/", methods=["GET"])
def home():
    return "Cloud ML Performance API is running"


# -------------------------------
# Prediction route
# -------------------------------
@app.route("/predict", methods=["POST"])
def predict():
    try:
        # Get JSON data from request
        data = request.get_json()

        # Read input values safely
        score = float(data.get("score", 0))
        response_time = float(data.get("response_time", 0))
        attempts = int(data.get("attempts", 0))

        # ML prediction
        prediction = model.predict([[score, response_time, attempts]])[0]

        # Convert numpy type → int
        prediction = int(prediction)

        # Map prediction to feedback
        if prediction == 2:
            performance = "Good"
        elif prediction == 1:
            performance = "Average"
        else:
            performance = "Poor"

        # Send response
        return jsonify({
            "performance": performance
        })

    except Exception as e:
        return jsonify({
            "error": str(e)
        }), 500


# -------------------------------
# Run app (Render compatible)
# -------------------------------
if __name__ == "__main__":
    app.run(host="0.0.0.0", port=10000)
