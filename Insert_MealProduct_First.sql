INSERT INTO MealProduct VALUES (
(SELECT MealId FROM Meals WHERE MealName = 'Sandwich'),
(SELECT ProdId FROM Products WHERE ProdName = 'Bread'),
50),
(
(SELECT MealId FROM Meals WHERE MealName = 'Sandwich'),
(SELECT ProdId FROM Products WHERE ProdName = 'Cheese'),
20),
(
(SELECT MealId FROM Meals WHERE MealName = 'Sandwich'),
(SELECT ProdId FROM Products WHERE ProdName = 'Sousage'),
30);
