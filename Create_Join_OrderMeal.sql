CREATE TABLE OrderMeals
(
    OrderId INT NOT NULL,
    MealId INT NOT NULL,
    Amount INT DEFAULT 1 NOT NULL,
    PRIMARY KEY (OrderId, MealId),
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (MealId) REFERENCES Meals(MealId)
);
