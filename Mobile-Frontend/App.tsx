import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import HomeScreen from './src/screens/HomeScreen';
import EmployeeDetailScreen from './src/screens/EmployeeDetailScreen';
import EmployeeEditScreen from './src/screens/EmployeeEditScreen';

export type RootStackParamList = {
  Home: undefined;
  EmployeeDetail: { id: string };
  EmployeeEdit: { id?: string };
};

const Stack = createNativeStackNavigator<RootStackParamList>();

function App(): React.JSX.Element {
  return (
    <NavigationContainer>
      <Stack.Navigator
        initialRouteName="Home"
        screenOptions={{
          headerStyle: {
            backgroundColor: '#F2F2F7',
          },
          headerShadowVisible: false,
          headerTintColor: '#007AFF',
          headerTitleStyle: {
            fontWeight: '600',
            color: '#000000',
          },
          contentStyle: { backgroundColor: '#F2F2F7' }
        }}
      >
        <Stack.Screen 
          name="Home" 
          component={HomeScreen} 
          options={{ title: 'Employees', headerLargeTitle: true }} 
        />
        <Stack.Screen 
          name="EmployeeDetail" 
          component={EmployeeDetailScreen} 
          options={{ title: 'Details' }} 
        />
        <Stack.Screen 
          name="EmployeeEdit" 
          component={EmployeeEditScreen} 
          options={{ title: 'Edit Employee', presentation: 'modal' }} 
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
}

export default App;
