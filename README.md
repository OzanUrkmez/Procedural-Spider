# Procedural Spider
 
A project that aims to procedurally create spider objects with procedural animations and different behaviours, all with native code other than Unity.

# Implemented features

## IK

The project currently has a custom IK solution written with C#. It also has a simple shader written to help interact with the IK target and pole target by making them visible even if they are behind other objects. 

It is thus possible to move around the spider leg procedurally with this algorithm and interface. 

![yes](https://user-images.githubusercontent.com/28452220/105067805-299f5e00-5a91-11eb-9b94-8cdce34945ef.gif)

The number of iterations to make the IK solution more accurate can be specified, as well as the acceptable margin of error after which to stop running the algorithm. 
