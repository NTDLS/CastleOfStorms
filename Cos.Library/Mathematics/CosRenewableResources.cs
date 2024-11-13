using Cos.Library.ExtensionMethods;

namespace Cos.Library.Mathematics
{
    /// <summary>
    /// Keeps track of a collection of time-renewable-resources such as "boost amount" or health.
    /// </summary>
    public class CosRenewableResources
    {
        #region class:RenewableResource.

        /// <summary>
        /// Keeps track of time-renewable-resources such as "boost amount".
        /// </summary>
        public class RenewableResource
        {
            /// <summary>
            /// When a resource reaches 0, it must renew bac to to this value before it can be used.
            /// </summary>
            public float CooldownFloor { get; set; } = 0;
            public float RebuildRatePerSecond { get; set; }
            public float MaxValue { get; set; }

            private bool _isCoolingDown = false;
            /// <summary>
            /// Returns true if the value is cooling down and is under the cooldown floor.
            /// </summary>
            public bool IsCoolingDown
            {
                get
                {
                    if (_availableResource >= CooldownFloor)
                    {
                        _isCoolingDown = false;
                    }
                    return _isCoolingDown;
                }
            }

            private float _availableResource = 0;
            public float AvailableResource
            {
                get => IsCoolingDown == true ? 0 : _availableResource;
                private set => _availableResource = value;
            }

            /// <summary>
            /// The resource value despite the cooldown floor.
            /// </summary>
            public float RawAvailableResource => _availableResource;

            /// <summary>
            /// Initializes the renewable resources.
            /// </summary>
            /// <param name="maxValue">The maximum value the resource can build to.</param>
            /// <param name="startingValue">The starting value of the resource.</param>
            /// <param name="RebuildRatePerSecond">The amount of resource to add per-second.</param>
            public RenewableResource(float maxValue, float startingValue, float rebuildRatePerSecond)
            {
                MaxValue = maxValue;
                AvailableResource = startingValue;
                RebuildRatePerSecond = rebuildRatePerSecond;
            }

            /// <summary>
            /// Initializes the renewable resources.
            /// </summary>
            /// <param name="maxValue">The maximum value the resource can build to.</param>
            /// <param name="startingValue">The starting value of the resource.</param>
            /// <param name="RebuildRatePerSecond">The amount of resource to add per-second.</param>
            /// <param name="cooldownFloor"> When a resource reaches 0, it must renew bac to to this value before it can be used.</param>
            public RenewableResource(float maxValue, float startingValue, float rebuildRatePerSecond, float cooldownFloor)
            {
                MaxValue = maxValue;
                AvailableResource = startingValue;
                RebuildRatePerSecond = rebuildRatePerSecond;
                CooldownFloor = cooldownFloor;
            }


            /// <summary>
            /// Consumes a given amount of resource.
            /// </summary>
            /// <param name="amount"></param>
            /// <returns></returns>
            public float Consume(float amount)
            {
                if (AvailableResource > amount)
                {
                    //Take the requested amount of given resource.
                    AvailableResource -= amount;
                }
                else
                {
                    //Deplete the remaining amount.
                    amount = AvailableResource;
                    AvailableResource = 0;
                }

                if (AvailableResource <= 0 && CooldownFloor > 0)
                {
                    _isCoolingDown = true;
                }

                return amount;
            }

            /// <summary>
            /// Accumulates new resources given the resources newable rate.
            /// </summary>
            public void RenewResource(float epoch)
            {
                float addedResource = epoch * RebuildRatePerSecond / 1000.0f;
                _availableResource += addedResource;
                AvailableResource = _availableResource.Clamp(0, MaxValue);
            }
        }

        #endregion

        public Dictionary<string, RenewableResource> Resources { get; set; } = new();

        public void Create(string key, RenewableResource renewableResource)
            => Resources.Add(key.ToLower(), renewableResource);

        /// <summary>
        /// Creates a new renewable resources and adds it to the resource collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="maxValue"></param>
        /// <param name="startingValue"></param>
        /// <param name="rebuildRatePerSecond"></param>
        public void Create(string key, float maxValue, float startingValue, float rebuildRatePerSecond)
            => Resources.Add(key.ToLower(), new RenewableResource(maxValue, startingValue, rebuildRatePerSecond));

        /// <summary>
        /// Creates a new renewable resources and adds it to the resource collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="maxValue"></param>
        /// <param name="startingValue"></param>
        /// <param name="rebuildRatePerSecond"></param>
        /// <param name="cooldownFloor"> When a resource reaches 0, it must renew bac to to this value before it can be used.</param>
        public void Create(string key, float maxValue, float startingValue, float rebuildRatePerSecond, float cooldownFloor)
            => Resources.Add(key.ToLower(), new RenewableResource(maxValue, startingValue, rebuildRatePerSecond, cooldownFloor));

        /// <summary>
        /// Gets the instance of the renewable resource.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public RenewableResource Snapshot(string key) => Resources[key.ToLower()];

        /// <summary>
        /// Consumes a given amount of resource.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public float Consume(string key, float amount) => Resources[key.ToLower()].Consume(amount);

        /// <summary>
        /// Gets the value of a resource but does not consume it.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float Observe(string key) => Resources[key.ToLower()].AvailableResource;

        /// <summary>
        /// Returns true if the value is under the cooldown floor.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsCoolingDown(string key) => Resources[key.ToLower()].IsCoolingDown;

        /// <summary>
        /// Gets the value of a resource despite the cooldown floor but does not consume it.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float ObserveRaw(string key) => Resources[key.ToLower()].RawAvailableResource;

        /// <summary>
        /// Accumulates new resources given each resources renewal rate.
        /// </summary>
        public void RenewAllResources(float epoch)
        {
            foreach (var item in Resources)
            {
                item.Value.RenewResource(epoch);
            }
        }
    }
}
