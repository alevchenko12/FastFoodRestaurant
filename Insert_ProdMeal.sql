INSERT INTO MealProduct VALUES (
(SELECT MealId FROM Meals WHERE MealName = 'Omlet'),
(SELECT ProdId FROM Products WHERE ProdName = 'Egg'),
100),
(
(SELECT MealId FROM Meals WHERE MealName = 'Omlet'),
(SELECT ProdId FROM Products WHERE ProdName = 'Yoghurt'),
50),
(
(SELECT MealId FROM Meals WHERE MealName = 'Dessert'),
(SELECT ProdId FROM Products WHERE ProdName = 'Banana'),
45),
(
(SELECT MealId FROM Meals WHERE MealName = 'Dessert'),
(SELECT ProdId FROM Products WHERE ProdName = 'Apple'),
45),
(
(SELECT MealId FROM Meals WHERE MealName = 'Dessert'),
(SELECT ProdId FROM Products WHERE ProdName = 'Yoghurt'),
100),
(
(SELECT MealId FROM Meals WHERE MealName = 'Breakfast'),
(SELECT ProdId FROM Products WHERE ProdName = 'Egg'),
45),
(
(SELECT MealId FROM Meals WHERE MealName = 'Breakfast'),
(SELECT ProdId FROM Products WHERE ProdName = 'Bread'),
45),
(
(SELECT MealId FROM Meals WHERE MealName = 'Breakfast'),
(SELECT ProdId FROM Products WHERE ProdName = 'Apple'),
100);
