import pandas as pd
import joblib
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score, classification_report

# -------------------------------
# Load dataset
# -------------------------------
data = pd.read_csv("dataset/quiz_performance_dataset_200.csv")

# Features & label
X = data[["score", "response_time", "attempts"]]
y = data["label"]

# -------------------------------
# Split dataset
# -------------------------------
X_train, X_test, y_train, y_test = train_test_split(
    X, y, test_size=0.2, random_state=42
)

# -------------------------------
# Train Random Forest model
# -------------------------------
model = RandomForestClassifier(
    n_estimators=300,
    random_state=42
)

model.fit(X_train, y_train)

# -------------------------------
# Evaluate model
# -------------------------------
y_pred = model.predict(X_test)

accuracy = accuracy_score(y_test, y_pred)
print(f"\n✅ Model Accuracy: {accuracy * 100:.2f}%\n")

print("📊 Classification Report:")
print(classification_report(y_test, y_pred))

# -------------------------------
# Save model
# -------------------------------
joblib.dump(model, "performance_model.pkl")
print("\n✅ Model saved as performance_model.pkl")
