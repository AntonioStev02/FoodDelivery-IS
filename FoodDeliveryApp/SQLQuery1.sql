INSERT INTO dbo.Restaurants
    (Id, RestaurantName, RestaurantLocation, RestaurantImage, DeliveryTime, MinPriceForOrder, OnlinePayment, CashPayment)
VALUES 
    (NEWID(), 'Gourmet Bistro', '123 Flavor Street, Foodtown', 'images/gourmet_bistro.jpg', '30-45', 20, 1, 0);