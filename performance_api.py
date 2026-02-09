from flask import Flask, request, jsonify
import joblib

app = Flask(__name__)

# Load trained ML model
model = joblib.load("performance_model.pkl")

@app.route("/", methods=["GET"])
def home():
    return "ML API is running"

@app.route("/predict", methods=["POST"])
def predict():
    data = request.get_json()

    score = data.get("score", 0)
    response_time = data.get("response_time", 0)
    attempts = data.get("attempts", 0)

    # ML prediction
    prediction = model.predict([[score, response_time, attempts]])[0]

    # 🔥 FIX: convert numpy.int64 → int → string label
    prediction = int(prediction)

    # Optional mapping (recommended for feedback)
    if prediction == 2:
        result = "Good"
    elif prediction == 1:
        result = "Average"
    else:
        result = "Poor"

    return jsonify({
        "performance": result
    })

if __name__ == "__main__":
    app.run(debug=True)
