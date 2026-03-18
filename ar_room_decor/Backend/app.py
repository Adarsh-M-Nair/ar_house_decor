from flask import Flask
from flask_cors import CORS

from routes.recommendation_routes import recommendation_bp
from routes.cost_routes import cost_bp
from routes.diy_routes import diy_bp

app = Flask(__name__)
CORS(app)

app.register_blueprint(recommendation_bp)
app.register_blueprint(cost_bp)
app.register_blueprint(diy_bp)

@app.route("/")
def home():
    return "AR Room Decor Backend Running Successfully!"

if __name__ == "__main__":
    app.run(debug=True)