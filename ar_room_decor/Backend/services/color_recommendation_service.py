def recommend_colors(style):

    if style == "modern":
        return [
            (180, 130, 70),   # blue
            (200, 200, 200),  # grey
            (150, 180, 200)   # pastel
        ]

    elif style == "minimal":
        return [
            (220, 220, 220),
            (200, 210, 200),
            (240, 240, 240)
        ]

    else:
        return [
            (180, 180, 150),
            (200, 170, 120),
            (160, 140, 100)
        ]