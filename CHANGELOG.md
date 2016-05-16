# Change Log

All notable changes to this project will be documented in this file.
Particle Windows Cloud SDK adheres to [Semantic Versioning](http://semver.org/).

---

## [0.1.0](https://github.com/spark/particle-windows-sdk/releases/tag/v0.1.0) (2016-05-16)

* Add .NET Framework 4.5.2 library for use in non WinRT applications
* `ParticleCloud`
 * Add public properties `OAuthClientId` and `OAuthClientSecret` for setting OAuth client tokens
 * Add `SignupWithCustomerAsync`, `RequestPasswordResetForCustomerAsync`, and `CreateClaimCodeForOrganizationAsync`
* `ParticleDevice`
 * Add `SignalAsync` to send a signal to the device to shout rainbows
 * Add public properties `IsFlashing`, `KnownPlatformId`, and `KnownProductId`
 * Add **Digistump Oak**, **RedBear Duo**, and **Bluz DK** to the list of know devices

## [0.0.5](https://github.com/spark/particle-windows-sdk/releases/tag/v0.0.5) (2016-05-03)

* Initial Release
