CREATE TABLE MealProdut (
	MealProdId INT PRIMARY KEY IDENTITY(1,1),
	MealId INT,
	ProdId INT,
	FOREIGN KEY (MealId) REFERENCES  Meals(MealId),
	FOREIGN KEY (ProdId) REFERENCES Products(ProdId)
);