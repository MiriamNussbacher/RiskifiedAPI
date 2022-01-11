Hi!
Thanks for reading my assignment. 

The assignment is using .net 5.0 and requires vs 2019 version 16.8 or higher.
If this is an issue ( the skeleton on github was written in .net 3.1), I'll try to see how to work this out.

The GateWay includes:
 a Controller to receive the requests and handle the responses.

the controller will then send to a service which will handle the payment.

Since each company handles the payment in a different way, There is service base class, and different services classes for each company.
Based on the credit card company parameter in the controller, the  system will inject the right service class.

each derived class has to implement:
buildPaymentBody - to create the body based on specific company api.
handleResponse - to parse response based on specific company api.
it also has its own members:
api url, headername, and header value. all values come from a configuration file.

The service (base) class implements the sendPayment which actually posts the payment to the relevant server.

It implements the retry mechanism as well. 

Validation and error handling:

bad request:
I used entity validation to make sure all properties are required.
In case of a missing header I manually returned a BadRequest response.

InternalServerError:
I wrote a middleware which captures any request to the server and adds try and catch to the pipeline. this way any error will be handled, (at the moment return 500 and empty body as required).

Bonus:
I really wanted to do it, but felt the time was short, and I won't be able to do this in the time period that was left in a clean and smart way. remarks are left in the code so you know I tried :).

Thanks. Miriam

 
