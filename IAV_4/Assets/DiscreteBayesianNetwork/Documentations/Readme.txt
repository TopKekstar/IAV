Overview
===
This package contains an implementation of Discrete Bayesian Network in C#. 

Bayesian Network is a way to represent probability tables and relationships between them. It is usually used by AIs when uncertainties are involved. It allows an AI to reason with probability. For example, an AI can use Bayesian Network to calculate the probability of winning the player based on the evidence it observes.

Discrete Bayesian Network only accept nodes with a discrete distribution.

Version
===
1.0.0

Installation
===
There is no installation required for using the library except for the BaysianJsonParser class. The BaysianJsonParser class requires 
the SimpleJSON library (http://wiki.unity3d.com/index.php/SimpleJSON) to be able to run. However, the SimpleJSON library does not 
contain a licence and therefore it was not included in this package to avoid legal issues.

To use the BaysianJsonParser class, download the SimpleJSON.cs file from the link above and add it as a reference in the project. You 
can then uncomment the BaysianJsonParser class and use it in your project.

Usage
===
Please see the example scene.

Author
===
Junjie Chen (jacky.jjchen@gmail.com)

ChangeLog
===
1.0.0 Initial Implementation