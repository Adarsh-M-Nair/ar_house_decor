from flask import Flask
from flask_cors import CORS

from routes.recommendation_routes import recommendation_bp
from routes.cost_routes import cost_bp
from routes.diy_routes import diy_bp

# ✅ Correct import
from routes.wall_ai_routes import wall_ai_bp

app = Flask(__name__)
CORS(app)

# Existing modules
app.register_blueprint(recommendation_bp)
app.register_blueprint(cost_bp)
app.register_blueprint(diy_bp)

# ✅ AI Wall Module
app.register_blueprint(wall_ai_bp)


@app.route("/")
def home():
    return "AR Room Decor Backend Running Successfully!"


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000, debug=True)