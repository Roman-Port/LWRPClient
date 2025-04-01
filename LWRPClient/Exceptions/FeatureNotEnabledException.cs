using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Exceptions
{
    public class FeatureNotEnabledException : Exception
    {
        public FeatureNotEnabledException(string featureName) : base($"Unable to access {featureName} feature because it was not enabled when the connection was constructed.")
        {
            this.featureName = featureName;
        }

        private readonly string featureName;

        /// <summary>
        /// The name of the feature trying to be accessed.
        /// </summary>
        public string FeatureName => featureName;
    }
}
