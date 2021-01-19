# Procedural Spider
 
A project that aims to procedurally create spider objects with procedural animations and different behaviours, all with native code other than Unity.

# Implemented features

## IK

The project currently has a custom IK solution written with C#. It also has a simple shader written to help interact with the IK target and pole target by making them visible even if they are behind other objects. 

It is thus possible to move around the spider leg procedurally with this algorithm and interface. 


The number of iterations to make the IK solution more accurate can be specified, as well as the acceptable margin of error after which to stop running the algorithm. 
