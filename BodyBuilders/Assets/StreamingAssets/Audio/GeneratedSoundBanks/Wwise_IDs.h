/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID ATTACH = 180661998U;
        static const AkUniqueID BUTTONPRESS = 317641954U;
        static const AkUniqueID CRUMBLINGLEDGE = 3268819721U;
        static const AkUniqueID DEATH = 779278001U;
        static const AkUniqueID DETACH = 1695175984U;
        static const AkUniqueID DOORMOVES = 3612883293U;
        static const AkUniqueID DROPBOX = 255253217U;
        static const AkUniqueID ENTERFAN = 230734316U;
        static const AkUniqueID ENTERFANAREA = 2406234679U;
        static const AkUniqueID EXITFAN = 1587122176U;
        static const AkUniqueID FOOTSTEP = 1866025847U;
        static const AkUniqueID GYREDEACTIVATE = 1571093136U;
        static const AkUniqueID GYREDETECTION = 3261812143U;
        static const AkUniqueID GYRESLICE = 4149978840U;
        static const AkUniqueID HEAVYLANDING = 1962910015U;
        static const AkUniqueID JUMP = 3833651337U;
        static const AkUniqueID LANDING = 2548270042U;
        static const AkUniqueID PASSTHROUGHPLATFORM = 107258764U;
        static const AkUniqueID PICKUPBOX = 4186279120U;
        static const AkUniqueID ROLLING = 4227290872U;
        static const AkUniqueID SHOOT = 3038207054U;
        static const AkUniqueID STARTMUSIC = 3827058668U;
        static const AkUniqueID STOPMUSIC = 1917263390U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace GROUNDMATERIAL
        {
            static const AkUniqueID GROUP = 3072116243U;

            namespace STATE
            {
                static const AkUniqueID GRASS = 4248645337U;
                static const AkUniqueID METAL = 2473969246U;
                static const AkUniqueID ROCK = 2144363834U;
            } // namespace STATE
        } // namespace GROUNDMATERIAL

        namespace SPECIALARMPART
        {
            static const AkUniqueID GROUP = 876059847U;

            namespace STATE
            {
                static const AkUniqueID BASIC = 3340296461U;
                static const AkUniqueID CANNON = 2393348022U;
            } // namespace STATE
        } // namespace SPECIALARMPART

    } // namespace STATES

    namespace SWITCHES
    {
        namespace CORRUPTIONPRESENCE
        {
            static const AkUniqueID GROUP = 3029199433U;

            namespace SWITCH
            {
                static const AkUniqueID FALSE = 2452206122U;
                static const AkUniqueID TRUE = 3053630529U;
            } // namespace SWITCH
        } // namespace CORRUPTIONPRESENCE

        namespace MUSICINTENSITY
        {
            static const AkUniqueID GROUP = 1301299809U;

            namespace SWITCH
            {
                static const AkUniqueID ABSENT = 1633207570U;
                static const AkUniqueID BRIDGETOINSANE = 2928650311U;
                static const AkUniqueID BRIDGETOINTENSE = 381137553U;
                static const AkUniqueID CHILL = 4294400669U;
                static const AkUniqueID INSANE = 3719846035U;
                static const AkUniqueID INTENSE = 4223512837U;
                static const AkUniqueID LINEAR = 3616880742U;
                static const AkUniqueID TENSEMEDIUM = 2997931687U;
                static const AkUniqueID THREAT = 1959898597U;
                static const AkUniqueID UPBEATMEDIUM = 1872095099U;
            } // namespace SWITCH
        } // namespace MUSICINTENSITY

        namespace TERMINALINAREA
        {
            static const AkUniqueID GROUP = 2886848227U;

            namespace SWITCH
            {
                static const AkUniqueID FALSE = 2452206122U;
                static const AkUniqueID TRUE = 3053630529U;
            } // namespace SWITCH
        } // namespace TERMINALINAREA

        namespace WATERFALLINAREA
        {
            static const AkUniqueID GROUP = 3746022203U;

            namespace SWITCH
            {
                static const AkUniqueID FALSE = 2452206122U;
                static const AkUniqueID TRUE = 3053630529U;
            } // namespace SWITCH
        } // namespace WATERFALLINAREA

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID CORRUPTIONPROXIMITY = 3858031969U;
        static const AkUniqueID LOCATION = 1176052424U;
        static const AkUniqueID PLAYBACK_RATE = 1524500807U;
        static const AkUniqueID RPM = 796049864U;
        static const AkUniqueID SPEECHSIDECHAINING = 1380407587U;
        static const AkUniqueID SS_AIR_FEAR = 1351367891U;
        static const AkUniqueID SS_AIR_FREEFALL = 3002758120U;
        static const AkUniqueID SS_AIR_FURY = 1029930033U;
        static const AkUniqueID SS_AIR_MONTH = 2648548617U;
        static const AkUniqueID SS_AIR_PRESENCE = 3847924954U;
        static const AkUniqueID SS_AIR_RPM = 822163944U;
        static const AkUniqueID SS_AIR_SIZE = 3074696722U;
        static const AkUniqueID SS_AIR_STORM = 3715662592U;
        static const AkUniqueID SS_AIR_TIMEOFDAY = 3203397129U;
        static const AkUniqueID SS_AIR_TURBULENCE = 4160247818U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID CHARACTER = 436743010U;
        static const AkUniqueID ENEMIES = 2242381963U;
        static const AkUniqueID ENVIRONMENT = 1229948536U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID PROJECTSOUNDBANK = 2110248291U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MOTION_FACTORY_BUS = 985987111U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID DEFAULT_MOTION_DEVICE = 4230635974U;
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
